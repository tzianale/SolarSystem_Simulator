using Models;

using TMPro;
using UnityEngine;
using SystemObject = System.Object;

namespace UI
{
    /// <summary>
    /// Extends the basic PlanetInfoPrefabController by implementing method GeneratePropertyDependingOnSubClass.
    /// This specific implementation replaces Text Fields with Input Fields for certain properties, allowing
    /// the user to tweak the description at will.
    /// </summary>
    public class PlanetVariableInfoPrefabController : PlanetInfoPrefabController
    {
        [SerializeField] private GameObject editablePropertiesFieldPrefab;
        [SerializeField] private TMP_InputField planetDescriptionContainer;

        /// <summary>
        /// Sets a description string to the Info Prefab
        /// </summary>
        /// 
        /// <param name="planetDescription">
        /// The string to set as Planet Description
        /// </param>
        protected override void SetDescription(string planetDescription)
        {
            planetDescriptionContainer.text = planetDescription;
        }

        private protected override GameObject GeneratePropertyDependingOnSubClass(string propertyName,
            string propertyValue, string measurementUnit)
        {
            return GenerateInputFieldUsingPrefab(propertyName, propertyValue, measurementUnit);
        }

        private GameObject GenerateInputFieldUsingPrefab(string propertyName, string propertyValue,
            string measurementUnit)
        {
            var planetPropertyGameObject = Instantiate(editablePropertiesFieldPrefab);

            var planetPropertyController = planetPropertyGameObject.GetComponent<PropertyFieldController>();
            
            planetPropertyController.AddListenerToAllOnSelection(_ => CameraControl.SetKeyboardLock(true));
            planetPropertyController.AddListenerToAllOnEditEnd(_ => CameraControl.SetKeyboardLock(false));

            var propertyText = new SystemObject[]
                { propertyName + NameValueSeparator, propertyValue, ValueUnitSeparatorForProperties + measurementUnit };

            planetPropertyController.SetText(propertyText);

            return planetPropertyGameObject;
        }

        /// <summary>
        /// Generates a new empty field for an additional Property at runtime
        /// </summary>
        public void AddNewEmptyStaticProperty()
        {
            var emptyProperty =
                GeneratePropertyDependingOnSubClass(UnknownPropertyText, UnknownPropertyText, UnknownPropertyText);

            emptyProperty.transform.SetParent(planetStaticPropertiesContainer.transform, false);

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}