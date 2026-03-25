using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using TMPro;
using System.Collections;
using System.Linq;
using UI;
using System;

namespace Tests.PlayMode
{
    /// <summary>
    /// Test fixture for play mode tests of the DropdownInputCelestialBodyType class.
    /// </summary>
    public class DropdownInputCelestialBodyTypeTests
    {
        private GameObject gameObject;
        private TMP_Dropdown dropdown;

        /// <summary>
        /// Sets up the test environment by creating a new GameObject and adding the required components.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            gameObject = new GameObject();
            dropdown = gameObject.AddComponent<TMP_Dropdown>();
            gameObject.AddComponent<DropdownInputCelestialBodyType>();
        }

        /// <summary>
        /// Tests that the dropdown is populated with the celestial body types after initialization.
        /// </summary>
        [UnityTest]
        public IEnumerator DropdownIsPopulatedAfterInitialization()
        {
            yield return null;
            Assert.IsNotNull(dropdown, "Dropdown component is not attached.");

            string[] expectedOptions = Enum.GetNames(typeof(Models.CelestialBodyType));
            bool allOptionsPresent = expectedOptions.All(expected => dropdown.options.Select(option => option.text).Contains(expected));

            Assert.IsTrue(allOptionsPresent, "Not all celestial body types are present in the dropdown.");
        }

        /// <summary>
        /// Cleans up the objects created for the test to prevent memory leaks and test cross-contamination.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
