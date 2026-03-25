using Models;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// Tests the Planet List Animator class to ensure that it works correctly
    /// </summary>
    public class PlanetListAnimatorTests
    {
        private GameObject _containerGameObject;
        private PlanetListAnimator _animator;

        private const string AnimatorInitText = "↑";

        /// <summary>
        /// Sets up the testing environment
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _containerGameObject = new GameObject();

            var containerGameObjectRecTransform = _containerGameObject.AddComponent<RectTransform>();

            _animator = _containerGameObject.AddComponent<PlanetListAnimator>();

            _animator.buttonText = new GameObject("TextObject").AddComponent<TextMeshProUGUI>();
            _animator.buttonText.text = AnimatorInitText; // Initialize text 

            // Directly use the existing transform rather than assigning a new one
            _animator.PlanetListContainerTransform = containerGameObjectRecTransform;
        }

        /// <summary>
        /// Tests the opening / closing of the list by ensuring that ListIsOpen
        /// and ButtonText parameters are set as expected
        /// </summary>
        [Test]
        public void OnArrowClick_TogglesListAndChangesButtonText()
        {
            // Act
            _animator.OnArrowClick();

            // Assert
            //Panel is opening
            Assert.IsTrue(_animator.ListIsOpen, "List should be open after first arrow click.");
            Assert.AreEqual("↓", _animator.buttonText.text, "Button text should be '↓' after first arrow click.");


            // Act
            _animator.OnArrowClick();

            // Assert
            //Panel is closing
            Assert.IsFalse(_animator.ListIsOpen, "List should be not open after second arrow click.");
            Assert.AreEqual("↑", _animator.buttonText.text, "Button text should be '↑' after second arrow click.");
        }

        /// <summary>
        /// Cleans up the testing environment after the tests have ended
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_containerGameObject);
        }
    }
}