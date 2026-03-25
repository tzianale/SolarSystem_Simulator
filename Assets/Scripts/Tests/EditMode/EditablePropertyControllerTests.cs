using NUnit.Framework;
using UnityEngine;
using TMPro;
using SystemObject = System.Object;
using Models;

namespace Tests.EditMode
{
    /// <summary>
    /// Test fixture for testing the EditablePropertyController class.
    /// </summary>
    public class EditablePropertyControllerTests
    {
        private GameObject gameObject;
        private EditablePropertyController controller;
        private TMP_InputField inputField;

        /// <summary>
        /// Sets up the test environment by creating a new GameObject and adding the required components.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Setup the game object and add required components
            gameObject = new GameObject();
            inputField = gameObject.AddComponent<TMP_InputField>();
            controller = gameObject.AddComponent<EditablePropertyController>();

            // Use reflection or direct assignment to set private fields if necessary
            var field = typeof(EditablePropertyController).GetField("planetPropertyText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(controller, inputField);
        }

        /// <summary>
        /// Tests that the SetText method sets the text of the TMP_InputField correctly.
        /// </summary>
        [Test]
        public void SetText_SetsInputFieldText()
        {
            // Arrange
            SystemObject[] texts = { "Hello", ", ", "World" };

            // Act
            controller.SetText(texts);

            // Assert
            Assert.AreEqual("Hello, World", inputField.text);
        }

        /// <summary>
        /// Tests that the DestroyProperty method destroys the GameObject.
        /// </summary>
        [Test]
        public void DestroyProperty_DestroysGameObject()
        {
            // Act
            controller.DestroyProperty();

            // Assert
            Assert.IsTrue(gameObject == null);
        }
    }
}
