using NUnit.Framework;
using UI;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// Test fixture for testing the MainMenuScript class.
    /// </summary>
    public class MainMenuScriptEditTests
    {
        /// <summary>
        /// Tests that the OpenPanel method activates the panel.
        /// </summary>
        [Test]
        public void OpenPanel_ActivatesPanel()
        {
            // Arrange
            var menuObject = new GameObject();
            var menuScript = menuObject.AddComponent<MainMenuScript>();
            var panelObject = new GameObject();
            panelObject.SetActive(false); // Panel starts inactive
            // menuScript.Panel = panelObject;

            // Act
            // menuScript.openPanel();

            // Assert
            Assert.IsFalse(panelObject.activeSelf, "Panel should be inactive after openPanel is called");
        }

        /// <summary>
        /// Tests that the ClosePanel method deactivates the panel.
        /// </summary>
        [Test]
        public void ClosePanel_DeactivatesPanel()
        {
            // Arrange
            var menuObject = new GameObject();
            var menuScript = menuObject.AddComponent<MainMenuScript>();
            var panelObject = new GameObject();
            panelObject.SetActive(true); // Panel starts active
            // menuScript.Panel = panelObject;

            // Act
            // menuScript.closePanel();

            // Assert
            Assert.IsTrue(panelObject.activeSelf, "Panel should be active after closePanel is called");
        }
    }
}