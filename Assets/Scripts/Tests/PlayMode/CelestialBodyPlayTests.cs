using System.Collections;
using Models;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;

namespace Tests.PlayMode
{
    /// <summary>
    /// Test fixture for play mode tests of the CelestialBody class.
    /// </summary>
    public class CelestialBodyPlayTests
    {
        private GameObject sun;
        private GameObject planet;
        private GameObject cameraControlObject;
        private CelestialBody sunCB;
        private CelestialBody planetCB;
        private CameraControlV2 cameraControl;
        private GameObject eventSystem;

        /// <summary>
        /// Sets up the test environment by initializing the required GameObjects and components.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            SimulationModeState.currentSimulationMode = SimulationModeState.SimulationMode.Sandbox;

            // Ensure an EventSystem exists in the scene
            if (EventSystem.current == null)
            {
                eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            // Setup Sun
            sun = new GameObject("Sun");
            sunCB = sun.AddComponent<CelestialBody>();
            sunCB.SetCelestialBodyType(CelestialBodyType.Sun);
            sunCB.SetMass(332900);
            var sunRb = sun.AddComponent<Rigidbody>();
            sunRb.useGravity = false;
            sunRb.isKinematic = true;
            sun.AddComponent<MeshRenderer>();
            sun.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            sun.AddComponent<LineRenderer>();

            // Setup Planet
            planet = new GameObject("Planet");
            planetCB = planet.AddComponent<CelestialBody>();
            planet.transform.position = new Vector3(755, 0, 0);
            planetCB.SetCelestialBodyType(CelestialBodyType.Planet);
            planetCB.SetMass(1);
            var planetRb = planet.AddComponent<Rigidbody>();
            planetRb.useGravity = false;
            planetRb.isKinematic = false;
            planet.AddComponent<MeshRenderer>();
            planet.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            planet.AddComponent<LineRenderer>();

            // Setup CameraControl
            cameraControlObject = new GameObject("CameraControl");
            var camHolder = new GameObject("CamHolder");
            var camera = new GameObject("MainCamera").AddComponent<Camera>();
            camHolder.transform.position = new Vector3(0, 0, -10);
            camera.transform.parent = camHolder.transform;

            cameraControl = cameraControlObject.AddComponent<CameraControlV2>();
            cameraControl.SetSun(sun);
            cameraControl.SetCamera(camera);
            cameraControl.SetCamHolder(camHolder);
            cameraControl.SetNeptune(planet);
            cameraControl.SetFollowTarget(planet.transform);

            sunCB.cameraControl = cameraControl;
            planetCB.cameraControl = cameraControl;
        }

        /// <summary>
        /// Tests that the celestial bodies update their velocity and position due to gravity.
        /// </summary>
        [UnityTest]
        public IEnumerator CelestialBodies_UpdateVelocityAndPositionDueToGravity()
        {
            float initialDistance = Vector3.Distance(sun.transform.position, planet.transform.position);

            // Act
            yield return new WaitForSecondsRealtime(2f); // Wait 2 seconds of real time

            // Assert
            float newDistance = Vector3.Distance(sun.transform.position, planet.transform.position);
            Assert.Less(newDistance, initialDistance, "Planet should move closer to the sun due to gravitational attraction.");
        }

        /// <summary>
        /// Cleans up the objects created for the test to prevent memory leaks and test cross-contamination.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            if (sun != null)
                Object.DestroyImmediate(sun);
            if (planet != null)
                Object.DestroyImmediate(planet);
            if (cameraControlObject != null)
                Object.DestroyImmediate(cameraControlObject);
            if (eventSystem != null)
                Object.DestroyImmediate(eventSystem);
        }
    }
}
