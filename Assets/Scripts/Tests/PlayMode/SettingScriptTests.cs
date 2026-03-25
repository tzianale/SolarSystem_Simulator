using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System.Collections;
using Models;

namespace Tests.PlayMode
{
    /// <summary>
    /// Test fixture for play mode tests of the SettingScript class.
    /// </summary>
    public class SettingScriptTests : MonoBehaviour
    {
        private GameObject testGameObject;

        /// <summary>
        /// Sets up the test environment by creating a new GameObject.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            testGameObject = new GameObject();
        }

        /// <summary>
        /// Tests that the loadScene method correctly loads the scene.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLoadScene()
        {
            var settingScript = testGameObject.AddComponent<SettingScript>();

            settingScript.loadScene();

            yield return null;

            Assert.IsTrue(SceneManager.GetSceneAt(0).isLoaded, "Die Szene 0 wird nicht geladen.");
        }

        /// <summary>
        /// Cleans up the objects created for the test to prevent memory leaks and test cross-contamination.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
            {
                GameObject.Destroy(testGameObject);
            }
        }
    }
}
