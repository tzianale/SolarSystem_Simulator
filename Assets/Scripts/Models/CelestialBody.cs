using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    /// <summary>
    /// Represents a celestial body in the simulation, handling its physical properties, 
    /// orbital mechanics, and interactions with other celestial bodies.
    /// </summary>
    public class CelestialBody : MonoBehaviour
    {
        [SerializeField] private CelestialBodyType celestType;
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float mass;
        [SerializeField] private float orbitRadius;
        [SerializeField] private float ratioToEarthYear = 1;
        private static List<CelestialBody> celestialBodies;
        [SerializeField] private float sideRealRotationPeriod;

        // Kepler Parameters
        [SerializeField] private float orbitalPeriod;
        [SerializeField] private float obliquityToOrbit;

        public CameraControlV2 cameraControl;

        // Constants
        private const float scaleFactor = 1000f;

        private void Start()
        {
            if (celestialBodies == null)
                celestialBodies = new List<CelestialBody>(FindObjectsOfType<CelestialBody>());
            else if (!celestialBodies.Contains(this))
                celestialBodies.Add(this);
            transform.Rotate(Vector3.right, obliquityToOrbit);


            if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer && celestType != CelestialBodyType.Sun)
            {
                sideRealRotationPeriod = 360f / sideRealRotationPeriod;
                UpdatePositionUsingAccurateKepler();
                UpdateRotation(true, (float)GameStateController.GetExplorerModeDay());
                transform.Rotate(Vector3.up, 80f, Space.Self);
            }
            else
            {
                // Multiply the rotation period by a factor, so planets rotate in sandbox mode visibly, but not realisticly.
                sideRealRotationPeriod = sideRealRotationPeriod * 36;
            }

            InitializeOrbitLine(transform);
        }

        private void FixedUpdate()
        {
            if (!GameStateController.GetIsPaused())
            {
                if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Sandbox)
                {
                    UpdateSandboxMode();
                }
                else
                {
                    UpdateExplorerMode();
                }
            }
        }

        private void UpdateSandboxMode()
        {
            foreach (CelestialBody planet in celestialBodies)
            {
                if (planet != this)
                {
                    UpdateVelocity(planet);

                }
            }

            UpdatePosition();
            UpdateRotation();
            UpdateOrbitalLineWidth();
        }

        private void UpdateExplorerMode()
        {
            if (celestType != CelestialBodyType.Sun)
            {
                UpdatePositionUsingAccurateKepler();
                UpdateRotation(true, (float)GameStateController.GetCurrentExplorerTimeStep());
                UpdateOrbitalLineWidth();
            }
        }

        private void UpdateVelocity(CelestialBody planet)
        {
            float gravitationalConstant = 10000000000f;
            float rSqr = (planet.transform.position - transform.position).sqrMagnitude;
            Vector3 forceDir = (planet.transform.position - transform.position).normalized;
            Vector3 force = gravitationalConstant * mass * planet.mass * forceDir / rSqr;
            velocity += force * (float)Math.Pow(10, -12) / mass;
        }

        private void UpdatePosition()
        {
            transform.position += velocity;
        }

        private void UpdateRotation(bool isExplorerMode = false, float currentExplorerTimeStep = 0)
        {

            if (isExplorerMode)
            {
                transform.Rotate(Vector3.up, -sideRealRotationPeriod * currentExplorerTimeStep * 24, Space.Self);
            }
            else
            {
                transform.Rotate(Vector3.up, -sideRealRotationPeriod * Time.deltaTime, Space.Self);
            }
        }

        private void UpdatePositionUsingAccurateKepler()
        {
            if (!PlanetDatabase.Planets.ContainsKey(gameObject.name))
            {
                Debug.LogError(gameObject.name + "This planet does not exist!");
                return;
            }

            var data = PlanetDatabase.Planets[gameObject.name];
            float currentTime = (float)GameStateController.GetExplorerModeDay();
            Vector3 position = ComputePlanetPosition(currentTime, data.Elements, data.Rates, data.ExtraTerms);
            transform.position = position;
        }

        private Vector3 ComputePlanetPosition(float currentTime, float[] elements, float[] rates, float[] extraTerms)
        {
            const float toRad = Mathf.PI / 180;
            float T = currentTime / 36525.0f;

            // Step 1: Compute Orbital Elements
            float a = elements[0] + rates[0] * T;
            float e = elements[1] + rates[1] * T;
            float I = elements[2] + rates[2] * T;
            float L = elements[3] + rates[3] * T;
            float w = elements[4] + rates[4] * T;
            float O = elements[5] + rates[5] * T;

            // Step 2: Compute Mean Anomaly
            float ww = w - O;
            float M = L - w;
            if (extraTerms.Length > 0)
            {
                float b = extraTerms[0];
                float c = extraTerms[1];
                float s = extraTerms[2];
                float f = extraTerms[3];
                M = L - w + b * T * T + c * Mathf.Cos(f * T * toRad) + s * Mathf.Sin(f * T * toRad);
            }

            M %= 360;
            float E = M + 57.29578f * e * Mathf.Sin(M * toRad);
            float dE = 1;
            int n = 0;
            while (Mathf.Abs(dE) > 1e-7f && n < 10)
            {
                dE = SolveKepler(M, e, E);
                E += dE;
                n++;
            }

            // Step 4: Compute Orbital Plane Coordinates
            float xp = a * (Mathf.Cos(E * toRad) - e);
            float yp = a * Mathf.Sqrt(1 - e * e) * Mathf.Sin(E * toRad);

            // Step 5: Transform to Ecliptic Coordinates
            I *= toRad;
            O *= toRad;
            ww *= toRad;
            float cosO = Mathf.Cos(O);
            float sinO = Mathf.Sin(O);
            float cosI = Mathf.Cos(I);
            float sinI = Mathf.Sin(I);
            float cosw = Mathf.Cos(ww);
            float sinw = Mathf.Sin(ww);

            float x = (cosw * cosO - sinw * sinO * cosI) * xp + (-sinw * cosO - cosw * sinO * cosI) * yp;
            float y = (cosw * sinO + sinw * cosO * cosI) * xp + (-sinw * sinO + cosw * cosO * cosI) * yp;
            float z = (sinw * sinI) * xp + (cosw * sinI) * yp;

            // Step 6: Correct Orientation
            // Final correction based on empirical observations
            // We rotate around the X axis to fix the inclination
            float angle = 90 * toRad; // degrees to radians
            float correctedY = y * Mathf.Cos(angle) - z * Mathf.Sin(angle);
            float correctedZ = y * Mathf.Sin(angle) + z * Mathf.Cos(angle);

            return new Vector3(x, correctedY, correctedZ) * scaleFactor; // Corrected orientation
        }


        private float SolveKepler(float M, float e, float E)
        {
            float toRad = Mathf.PI / 180;
            float dM = M - (E - e / toRad * Mathf.Sin(E * toRad));
            float dE = dM / (1 - e * Mathf.Cos(E * toRad));
            return dE;
        }

        private void InitializeOrbitLine(Transform meshTransform)
        {
            GameObject orbitLineGameObject = new GameObject("OrbitLine");
            orbitLineGameObject.transform.SetParent(meshTransform);

            LineRenderer lineRenderer = orbitLineGameObject.AddComponent<LineRenderer>();
            lineRenderer.loop = SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer;
            lineRenderer.widthMultiplier = 5.0f;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

            OrbitLineController orbitLineController = orbitLineGameObject.AddComponent<OrbitLineController>();
            orbitLineController.CelestialBody = this;

            Texture2D planetTexture = meshTransform.GetComponent<Renderer>().material.mainTexture as Texture2D;

            if (planetTexture != null && planetTexture.isReadable)
            {
                Color32 averageColor = AverageColorFromTexture(planetTexture);
                lineRenderer.startColor = averageColor;
                lineRenderer.endColor = averageColor;
            }
            else
            {
                Color meshColor = meshTransform.GetComponent<Renderer>().material.GetColor("_Color");
                lineRenderer.startColor = meshColor;
                lineRenderer.endColor = meshColor;
            }
        }

        private void UpdateOrbitalLineWidth()
        {
            if (cameraControl == null) return;
            float zoomScale = cameraControl.GetZoomScale();

            OrbitLineController olc = GetComponentInChildren<OrbitLineController>();
            olc.UpdateLineWidth(zoomScale);
        }

        /// <summary>
        /// Computes the points that define the orbit line in explorer mode.
        /// </summary>
        /// <returns>An array of Vector3 points representing the orbit line.</returns>
        public Vector3[] GetExplorerLinePoints()
        {
            int pointsLength = 360;
            Vector3[] points = new Vector3[pointsLength];

            if (celestType != CelestialBodyType.Sun)
            {
                if (!PlanetDatabase.Planets.ContainsKey(gameObject.name))
                {
                    Debug.LogError(gameObject.name + " not found in PlanetDatabase!");
                    return points;
                }
                var data = PlanetDatabase.Planets[gameObject.name];
                for (int i = 0; i < pointsLength; i++)
                {
                    float t = i / (float)pointsLength * orbitalPeriod;
                    points[i] = ComputePlanetPosition(t, data.Elements, data.Rates, data.ExtraTerms);
                }
            }

            return points;
        }

        private Color32 AverageColorFromTexture(Texture2D tex)
        {
            Color32[] texColors = tex.GetPixels32();
            int total = texColors.Length;

            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = 0; i < total; i++)
            {
                r += texColors[i].r;
                g += texColors[i].g;
                b += texColors[i].b;
            }

            return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 255);
        }

        //getter and setter methods
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetOrbitalPeriod(float period) // Add this method
        {
            orbitalPeriod = period;
        }

        public CelestialBodyType GetCelestialBodyType() => celestType;
        public void SetCelestialBodyType(CelestialBodyType type) => celestType = type;

        public float GetMass() => mass;
        public void SetMass(float value) => mass = value;

        public float GetOrbitRadius() => orbitRadius;
        public void SetOrbitRadius(float radius) => orbitRadius = radius;

        public float GetRatioToEarthYear() => ratioToEarthYear;
        public void SetRatioToEarthYear(float ratio) => ratioToEarthYear = ratio;

        public Vector3 GetVelocity() => velocity;
        public void SetVelocity(Vector3 velocity) => this.velocity = velocity;

        public List<CelestialBody> GetCelestialBodies() => celestialBodies;
        public void SetCelesitalBodies(List<CelestialBody> celestialBodies) => this.celestialBodies = celestialBodies;
    }
}