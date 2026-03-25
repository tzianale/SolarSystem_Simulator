using TMPro;
using Utils;
using Models;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

using SystemObject = System.Object;
using Button = UnityEngine.UI.Button;

namespace UI
{
    /// <summary>
    /// Provides useful methods to control, set and handle each Planet Info Tab
    /// </summary>
    public abstract class PlanetInfoPrefabController : MonoBehaviour
    {
        private protected const string UnknownPropertyText = "Unknown";
        private protected const string NameValueSeparator = " : ";
        private protected const string ValueUnitSeparatorForProperties = " ";
        
        private const string ValueUnitSeparatorForRawData = "_";
        
        [SerializeField] private protected GameObject variablePropertiesPrefab;
        
        
        [SerializeField] private protected TextMeshProUGUI planetNameField;

        [SerializeField] private protected GameObject planetSpriteField;

        [SerializeField] private Button hidePlanetButton;

        [SerializeField] private protected GameObject planetVariablePropertiesTag;
        [SerializeField] private protected GameObject planetVariablePropertiesContainer;

        [SerializeField] private protected GameObject planetStaticPropertiesContainer;

        [SerializeField] private protected GameObject planetLiveStatsContainer;

        [SerializeField] private protected int propertiesTextSize;

        [SerializeField] private Button closeTabButton;
        

        /// <summary>
        /// Getter method for the Button that closes this info tab
        /// </summary>
        public Button CloseTabButton => closeTabButton;

        private readonly List<ObservablePropertyController> _refreshableVariableTextFields = new();
        private readonly List<TextMeshProUGUI> _refreshableLiveTextFields = new();

        protected CameraControlV2 CameraControl;
        
        private Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> _variableProperties;
        
        private Dictionary<string, Func<string>> _liveStats;

        private bool _planetActive = true;
        
        /// <summary>
        /// Constructor-like method for initialising the fields of a Planet info tab
        /// </summary>
        /// 
        /// <param name="planetName">The name of the planet</param>
        /// <param name="planetSprite">The picture that will be used as icon for the planet</param>
        /// <param name="planetObject">The GameObject associated with the planet</param>
        /// <param name="cameraControl">The CameraControl script to lock when the properties are being edited</param>
        /// <param name="variableProperties">The Dictionary of properties that will update the simulation when changed by the user</param>
        /// <param name="planetStaticProperties">The Dictionary of properties that won't need to be updated and their values</param>
        /// <param name="planetLiveStats">The Dictionary of properties that will be updated and the methods to retrieve the updated data</param>
        /// <param name="planetDescription">The description of the planet</param>
        public void SetPlanetInfo(string planetName, Sprite planetSprite, GameObject planetObject, CameraControlV2 cameraControl,
            Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> variableProperties,
            Dictionary<string, string> planetStaticProperties, 
            Dictionary<string, Func<string>> planetLiveStats,
            string planetDescription)
        {
            planetNameField.text = planetName;
            planetSpriteField.GetComponent<Image>().sprite = planetSprite;

            CameraControl = cameraControl;

            _variableProperties = variableProperties;
            _liveStats = planetLiveStats;
            
            hidePlanetButton.onClick.AddListener(() =>
            {
                _planetActive = !_planetActive;
                planetObject.SetActive(_planetActive);
            });
            
            foreach (var propertyField in GenerateVariablePropertiesList())
            {
                propertyField.transform.SetParent(planetVariablePropertiesContainer.transform, false);
            }

            foreach (var propertyField in GenerateStaticPropertiesList(planetStaticProperties))
            {
                propertyField.transform.SetParent(planetStaticPropertiesContainer.transform, false);
            }

            foreach (var propertyField in GenerateLiveStatsList(_liveStats))
            {
                propertyField.transform.SetParent(planetLiveStatsContainer.transform, false);
            }

            SetDescription(planetDescription);
        }

        
        private List<GameObject> GenerateVariablePropertiesList()
        {
            var resultList = new List<GameObject>();
            
            if (_variableProperties.Count == 0)
            {
                planetVariablePropertiesTag.SetActive(false);
                planetVariablePropertiesContainer.SetActive(false);
            }
            else
            {
                foreach (var variableProperty in _variableProperties)
                {
                    var variablePropertyGameObject = Instantiate(variablePropertiesPrefab);

                    var variablePropertyController = variablePropertyGameObject.GetComponent<ObservablePropertyController>();

                    var propertyName = variableProperty.Key;
                    var propertyValueAndUnit = SeparateValueAndUnit(variableProperty.Value.FirstObject.Invoke());
                    
                    var propertyValue = propertyValueAndUnit.FirstObject;
                    var propertyUnit = propertyValueAndUnit.SecondObject;
                    
                    var propertyText = GenerateObservablePropertyText(propertyName, propertyValue, propertyUnit);

                    variablePropertyController.SetText(propertyText);
                    
                    variablePropertyController.AddListenerToAllOnSelection(_ => CameraControl.SetKeyboardLock(true));
                    variablePropertyController.AddListenerToAllOnEditEnd(_ => CameraControl.SetKeyboardLock(false));
                    
                    variablePropertyController.AddListenerToPropertyValueEditEnd(variableProperty.Value.SecondObject);

                    resultList.Add(variablePropertyGameObject);
                    _refreshableVariableTextFields.Add(variablePropertyController);
                }
            }

            return resultList;
        }


        private static SystemObject[] GenerateObservablePropertyText(string propertyName, string propertyValue, string propertyUnit)
        {
            return new SystemObject[] { propertyName + NameValueSeparator, propertyValue, ValueUnitSeparatorForProperties + propertyUnit};
        }

        /// <summary>
        /// Sets the description of this planet
        /// </summary>
        /// 
        /// <param name="planetDescription">The string that will be used as description</param>
        protected abstract void SetDescription(string planetDescription);
        

        private void Update()
        {
            RefreshLiveInfo();
            RefreshVariableInfo();
        }

        private void RefreshLiveInfo()
        {
            var newLiveStatsList = GenerateLiveStatsValues();

            for (var propertyIndex = 0; propertyIndex < _refreshableLiveTextFields.Count; propertyIndex++)
            {
                _refreshableLiveTextFields[propertyIndex].text = newLiveStatsList[propertyIndex];
            }
        }

        private void RefreshVariableInfo()
        {
            var newLiveStatsList = GenerateVariableStatsValues();

            for (var propertyIndex = 0; propertyIndex < _refreshableVariableTextFields.Count; propertyIndex++)
            {
                var currentController = _refreshableVariableTextFields[propertyIndex];

                if (!currentController.IsBeingEdited)
                {
                    currentController.SetText(newLiveStatsList[propertyIndex]);
                }
            }
        }

        private List<GameObject> GenerateStaticPropertiesList(Dictionary<string, string> planetProperties)
        {
            var planetPropertiesNames = planetProperties.Keys.ToArray();
            var planetPropertiesValues = planetProperties.Values.ToArray();

            var resultList = new List<GameObject>();
            
            var propertiesNamesCount = planetPropertiesNames.Count();
            var propertiesValuesCount = planetPropertiesValues.Count();
            
            var elementCount = GetMaxInt(propertiesNamesCount, propertiesValuesCount);

            for (var elementIndex = 0; elementIndex < elementCount; elementIndex++)
            {
                var propertyName = UnknownPropertyText;
                var propertyValueAndUnit = UnknownPropertyText;

                if (elementIndex < propertiesNamesCount) propertyName = planetPropertiesNames[elementIndex];
                if (elementIndex < propertiesValuesCount) propertyValueAndUnit = planetPropertiesValues[elementIndex];

                var propertyValueAndUnitSeparated = SeparateValueAndUnit(propertyValueAndUnit);

                var propertyValue = propertyValueAndUnitSeparated.FirstObject;
                var propertyUnit = propertyValueAndUnitSeparated.SecondObject;

                resultList.Add(GeneratePropertyDependingOnSubClass(propertyName, propertyValue, propertyUnit));
            }

            return resultList;
        }


        private static TwoObjectContainer<string, string> SeparateValueAndUnit(string inputString)
        {
            var propertyUnit = "";

            var valueAndUnitSeparated = inputString.Split(ValueUnitSeparatorForRawData);
                
            var propertyValue = valueAndUnitSeparated[0];

            if (valueAndUnitSeparated.Length > 1)
            { 
                propertyUnit = valueAndUnitSeparated[1];
            }

            return new TwoObjectContainer<string, string>(propertyValue, propertyUnit);
        }
        

        private List<GameObject> GenerateLiveStatsList(Dictionary<string, Func<string>> planetProperties)
        {
            var resultList = new List<GameObject>();

            foreach (var property in planetProperties)
            {
                resultList.Add(GeneratePropertySubClassIndependently(property.Key, property.Value()));
            }
            
            return resultList;
        }
        

        private List<string> GenerateLiveStatsValues()
        {
            var properties = new List<string>();

            foreach (var property in _liveStats)
            {
                var propertyName = property.Key;
                var propertyValue = property.Value();
                
                properties.Add(propertyName + NameValueSeparator + propertyValue);
            }

            return properties;
        }

        private List<SystemObject[]> GenerateVariableStatsValues()
        {
            var properties = new List<SystemObject[]>();

            foreach (var property in _variableProperties)
            {
                var propertyName = property.Key;
                var propertyRawValue = property.Value.FirstObject.Invoke();

                var propertyValueAndUnit = SeparateValueAndUnit(propertyRawValue);

                var propertyValue = propertyValueAndUnit.FirstObject;
                var propertyUnit = propertyValueAndUnit.SecondObject;
                
                properties.Add(GenerateObservablePropertyText(propertyName, propertyValue, propertyUnit));
            }

            return properties;
        }
        
        private GameObject GeneratePropertySubClassIndependently(string propertyName, string propertyValue)
        {
            var newListElement = new GameObject(propertyName + " Property");

            var contentSizeFitter = newListElement.AddComponent<ContentSizeFitter>();

            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
            var textElement = newListElement.AddComponent<TextMeshProUGUI>();
            
            textElement.text = propertyName + NameValueSeparator + propertyValue;
            textElement.fontSize = propertiesTextSize;

            _refreshableLiveTextFields.Add(textElement);
            
            return newListElement;
        }

        private protected abstract GameObject GeneratePropertyDependingOnSubClass(string propertyName, string propertyValue, string measurementUnit);

        private static int GetMaxInt(int firstNumber, int secondNumber)
        {
            return (firstNumber >= secondNumber) ? firstNumber : secondNumber;
        }
    }
}