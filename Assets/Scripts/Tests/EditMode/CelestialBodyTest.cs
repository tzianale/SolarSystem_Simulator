using Models;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// Test fixture for testing the CelestialBody class.
    /// </summary>
    public class CelestialBodyTest
    {
        private CelestialBody celestialBody;
        private GameObject celestialBodyObject;

        /// <summary>
        /// Sets up the test environment by creating a new GameObject and adding a CelestialBody component to it.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            celestialBodyObject = new GameObject();
            celestialBody = celestialBodyObject.AddComponent<CelestialBody>();
        }

        /// <summary>
        /// Tests the initialization of a CelestialBody as a Sun.
        /// </summary>
        [Test]
        public void CelestialBody_Initializes_SunCorrectly()
        {
            //Arrange
            var sunMass = 1.99e+22f;

            //Act
            celestialBody.SetCelestialBodyType(CelestialBodyType.Sun);
            celestialBody.SetMass(sunMass);
            celestialBody.SetOrbitRadius(0);
            celestialBody.SetRatioToEarthYear(1);

            //Assert
            Assert.AreEqual(CelestialBodyType.Sun, celestialBody.GetCelestialBodyType());
            Assert.AreEqual(sunMass, celestialBody.GetMass());
            Assert.AreEqual(0, celestialBody.GetOrbitRadius());
            Assert.AreEqual(1, celestialBody.GetRatioToEarthYear());
        }

        /// <summary>
        /// Tests the initialization of a CelestialBody as Earth.
        /// </summary>
        [Test]
        public void CelestialBody_Initializes_EarthCorrectly()
        {
            //Arrange
            var earthMass = 5.97217e+16f;

            //Act
            celestialBody.SetCelestialBodyType(CelestialBodyType.Planet);
            celestialBody.SetMass(earthMass);

            //Assert
            Assert.AreEqual(CelestialBodyType.Planet, celestialBody.GetCelestialBodyType());
            Assert.AreEqual(earthMass, celestialBody.GetMass());
        }

        /// <summary>
        /// Tests that the GetExplorerLinePoints method returns the correct number of points.
        /// </summary>
        [Test]
        public void GetOrbitLinePoints_ShouldReturnCorrectNumberOfPoints()
        {
            // Arrange
            celestialBody.SetOrbitRadius(360f); // Set a test orbit radius

            // Act
            Vector3[] points = celestialBody.GetExplorerLinePoints();

            // Assert
            Assert.AreEqual(360, points.Length, "GetOrbitLinePoints should return 360 points.");
        }

        /// <summary>
        /// Tears down the test environment by destroying the GameObject created for the test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(celestialBodyObject);
        }
    }
}
