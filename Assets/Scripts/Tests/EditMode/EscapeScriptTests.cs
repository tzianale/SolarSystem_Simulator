using NUnit.Framework;
using UI;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// Tests the correct implementation of the Escape Script
    /// </summary>
    public class EscapeScriptTests
    {
        private GameObject _panelObject;
        private EscapeScript _escapeScript;

        /// <summary>
        /// Sets up the testing environment
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var menuObject = new GameObject();
            _escapeScript = menuObject.AddComponent<EscapeScript>();
            _panelObject = new GameObject();
            _escapeScript.Panel = _panelObject;
        }

        /// <summary>
        /// Tests that a call to the OpenPanel method sets the object to active
        /// </summary>
        [Test]
        public void OpenPanel_ActivatesPanel()
        {
            // Arrange
            _panelObject.SetActive(false); // Panel starts inactive

            // Act
            _escapeScript.OpenPanel();

            // Assert
            Assert.IsTrue(_panelObject.activeSelf, "Panel should be active after openPanel is called");
        }
        
        /// <summary>
        /// Tests that a call to the ClosePanel method sets the object to inactive
        /// </summary>
        [Test]
        public void ClosePanel_DeactivatesPanel()
        {
            // Arrange
            _panelObject.SetActive(true); // Panel starts active

            // Act
            _escapeScript.ClosePanel();

            // Assert
            Assert.IsFalse(_panelObject.activeSelf, "Panel should be inactive after closePanel is called");
        }

        /// <summary>
        /// Cleans up the environment after tests have finished executing
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_panelObject);
            Object.DestroyImmediate(_escapeScript.gameObject);
        }
    }
}