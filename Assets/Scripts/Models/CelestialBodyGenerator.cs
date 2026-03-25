using System;
using System.Collections.Generic;
using UnityEngine;
using LightType = UnityEngine.LightType;

namespace Models
{
    /// <summary>
    /// Provides functionality to generate celestial bodies within the simulation.
    /// </summary>
    public class CelestialBodyGenerator : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private const string KeywordEmission = "_EMISSION";
        private const float SunLightRange = 100000.0f;
        private const float SunLightIntensity = 2.0f;
        private const float SunEmissionIntensity = 150.0f;

        /// <summary>
        /// Creates a new GameObject representing a celestial body and configures its properties.
        /// </summary>
        /// <param name="name">The name of the celestial body. Must not be null or empty.</param>
        /// <param name="type">The type of the celestial body (e.g., Planet, Sun).</param>
        /// <param name="position">The position of the celestial body in space.</param>
        /// <param name="mass">The mass of the celestial body. Must be greater than zero.</param>
        /// <param name="diameter">The diameter of the celestial body. Must be greater than zero.</param>
        /// <param name="velocity">The velocity of the celestial body.</param>
        /// <param name="color">The color of the celestial body.</param>
        /// <returns>The created GameObject representing the celestial body.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the name is null or empty, or if mass or diameter are less than or equal to zero.
        /// </exception>
        public static GameObject CreateNewCelestialBodyGameObject(string name, CelestialBodyType type, Vector3 position, float mass, float diameter, Vector3 velocity, Color color)
        {
            // Validate name
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            // Validate mass
            if (mass <= 0)
                throw new ArgumentException("Mass must be greater than zero.", nameof(mass));

            // Validate diameter
            if (diameter <= 0)
                throw new ArgumentException("Diameter must be greater than zero.", nameof(diameter));

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.localScale = new Vector3(diameter, diameter, diameter);
            gameObject.transform.position = position;
            gameObject.name = name;

            Material material = new Material(Shader.Find("Standard"));
            material.color = color;

            if (type == CelestialBodyType.Sun)
            {
                Light newPointLight = gameObject.AddComponent<Light>();
                newPointLight.type = LightType.Point;
                newPointLight.range = SunLightRange;
                newPointLight.intensity = SunLightIntensity;

                material.EnableKeyword(KeywordEmission);
                material.SetColor(EmissionColor, color * SunEmissionIntensity);
            }

            gameObject.GetComponent<Renderer>().material = material;

            AddNewCelestialBodyToGameObject(gameObject, type, velocity, mass);
            return gameObject;
        }

        private static void AddNewCelestialBodyToGameObject(GameObject gameObject,
            CelestialBodyType type, Vector3 velocity, float mass)
        {
            CelestialBody newBody = gameObject.AddComponent<CelestialBody>();

            newBody.SetMass(mass);
            newBody.SetVelocity(velocity);
            newBody.SetCelestialBodyType(type);
            List<CelestialBody> bodies = newBody.GetCelestialBodies() ?? new List<CelestialBody>(FindObjectsOfType<CelestialBody>());

            foreach (CelestialBody body in bodies)
            {
                body.SetCelesitalBodies(bodies);
            }

            // Ensure camHolder object exists and has the CameraControlV2 component
            GameObject camHolder = GameObject.Find("camHolder");
            if (camHolder != null)
            {
                CameraControlV2 cameraControl = camHolder.GetComponent<CameraControlV2>();
                if (cameraControl != null)
                {
                    newBody.cameraControl = cameraControl;
                }
                else
                {
                    Debug.LogError("CameraControlV2 component not found on camHolder.");
                }
            }
            else
            {
                Debug.LogError("camHolder object not found.");
            }
        }
    }
}