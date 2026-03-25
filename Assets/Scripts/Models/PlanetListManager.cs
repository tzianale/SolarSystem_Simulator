using UI;
using Utils;
using QuickOutline.Scripts;
using Models.PlanetListUtils;

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace Models
{
    /// <summary>
    /// Handles the Planet List by creating new Planet List Elements as well as Planet Info Tabs
    /// </summary>
    public class PlanetListManager : MonoBehaviour
    {
        private enum DataPropertyIndexes
        {
            PlanetName = 0,
            PlanetType = 2
        }

        private enum DataDescriptionIndexes
        {
            PlanetName,
            PlanetType,
            PlanetDescription
        }

        private const string PropertiesPath = "PlanetProperties.csv";
        private const string DescriptionsPath = "PlanetDescriptions.csv";

        private const string MoonDetector = "Rocky Moon";
        private const string DwarfDetector = "Dwarf Planet";

        private const string EarthsMoonName = "Earths Moon";

        private const string NullData = "null";
        private const string UnknownData = "good question!";

        [SerializeField]
        private float highlightWidth = 5f;

        [SerializeField]
        private Color highlightColor = Color.green;

        [SerializeField]
        private GameObject sun;

        [SerializeField]
        private Transform planetListContent;

        [SerializeField]
        private Transform canvas;

        [SerializeField]
        private GameObject planetObjectPrefab;

        [SerializeField]
        private GameObject planetInfoPrefab;

        [SerializeField]
        private List<Sprite> planetSprites;

        [SerializeField]
        private List<GameObject> planetModels;

        [SerializeField]
        private CameraControlV2 cameraControl;

        [SerializeField]
        private bool allowPropertyEditing;

        [SerializeField]
        private Color highlightingColor;


        private readonly Wrapper<GameObject> _activeInfoTab = new(null);
        private readonly Wrapper<GameObject> _highlightedPlanet = new(null);

        private readonly List<string> _planetNames = new();

        private bool _highlightedPlanetOriginalEmissionEnabled;
        private Color _highlightedPlanetOriginalEmissionValue;

        private void Start()
        {
            _highlightedPlanet.AddOnSetValueAction(HandlePlanetHighlighting);

            var propertiesData = CsvReader.ReadCsv(PropertiesPath);
            var descriptionsData = CsvReader.ReadCsv(DescriptionsPath);

            var planetProperties = UnpackPlanetDataFromCsv(propertiesData);
            var planetDescriptions = UnpackPlanetDescriptionsFromCsv(descriptionsData);

            if (planetSprites.Count == _planetNames.Count)
            {
                for (var i = 0; i < _planetNames.Count; i++)
                {
                    var currentPlanetModel = planetModels[i];

                    Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> variableProperties = new();

                    if (allowPropertyEditing)
                    {
                        variableProperties = PlanetListDictionaries.GetVariablePropertiesDictionary(currentPlanetModel);
                    }

                    var liveStats = PlanetListDictionaries.GetLiveStatsDictionary(currentPlanetModel, sun);

                    CreateNewPlanet(planetSprites[i], _planetNames[i], currentPlanetModel,
                        variableProperties,
                        planetProperties[i],
                        liveStats,
                        planetDescriptions[i]);
                }
            }
        }
        private void HandlePlanetHighlighting(GameObject oldPlanet, GameObject newPlanet)
        {
            if (oldPlanet)
            {
                var oldOutline = oldPlanet.GetComponent<Outline>();

                if (oldOutline)
                {
                    oldOutline.enabled = false;
                }
            }

            if (newPlanet)
            {
                var newRenderer = newPlanet.GetComponent<Outline>();

                if (!newRenderer)
                {
                    newRenderer = newPlanet.AddComponent<Outline>();
                }

                newRenderer.OutlineColor = highlightColor;
                newRenderer.OutlineWidth = highlightWidth;
                newRenderer.enabled = true;
            }
        }

        private void CreateNewPlanet(Sprite planetSprite, string planetName, GameObject planetObject,
            Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> variableProperties,
            Dictionary<string, string> staticProperties,
            Dictionary<string, Func<string>> liveStats,
            string planetDescription)
        {
            var planetListElement = Instantiate(planetObjectPrefab, planetListContent);
            var planetInfoTab = Instantiate(planetInfoPrefab, canvas);

            var planetListElementPrefabController = planetListElement.GetComponent<PlanetListElementPrefabController>();
            var planetInfoPrefabController = planetInfoTab.GetComponent<PlanetInfoPrefabController>();

            var planetInfoCloseButton = planetInfoPrefabController.CloseTabButton;

            var onClickActions = new List<Action<int>>
            {
                clickCount => planetListElementPrefabController.HandleClickEvent(clickCount)
            };

            SetGameObjectOnClickBehaviour(planetObject, onClickActions);

            planetListElementPrefabController.SetPlanetInfo(
                planetSprite, planetName, planetObject,
                cameraControl,
                planetInfoTab,
                _activeInfoTab,
                _highlightedPlanet,
                planetInfoCloseButton);

            planetInfoPrefabController.SetPlanetInfo(planetName, planetSprite, planetObject, cameraControl,
                variableProperties,
                staticProperties,
                liveStats,
                planetDescription);

            planetInfoTab.SetActive(false);
        }

        private static void SetGameObjectOnClickBehaviour(GameObject planetObject, List<Action<int>> actions)
        {
            var onClick = planetObject.AddComponent<OnGameObjectClick>();

            onClick.SetActions(actions);

            for (var childIndex = 0; childIndex < planetObject.transform.childCount; childIndex++)
            {
                var child = planetObject.transform.GetChild(childIndex).gameObject;

                onClick = child.AddComponent<OnGameObjectClick>();

                onClick.SetActions(actions);
            }
        }

        private List<Dictionary<string, string>> UnpackPlanetDataFromCsv(List<List<string>> data)
        {
            var result = new List<Dictionary<string, string>>();
            var labels = data[0];

            var rowCount = data.Count;

            for (var rowIndex = 1; rowIndex < rowCount; rowIndex++)
            {
                var row = data[rowIndex];

                if (IncludePlanetInfoInList(
                        row[(int)DataPropertyIndexes.PlanetName],
                        row[(int)DataPropertyIndexes.PlanetType]))
                {
                    var planetProperties = new Dictionary<string, string>();

                    for (var property = 0; property < row.Count; property++)
                    {
                        if ((DataPropertyIndexes)property == DataPropertyIndexes.PlanetName)
                        {
                            _planetNames.Add(row[property]);
                        }
                        else if (row[property] == NullData)
                        {
                            planetProperties.Add(labels[property], UnknownData);
                        }
                        else
                        {
                            planetProperties.Add(labels[property], row[property]);
                        }
                    }

                    result.Add(planetProperties);
                }
            }

            return result;
        }

        private List<string> UnpackPlanetDescriptionsFromCsv(List<List<string>> data)
        {
            var planetDescriptions = new List<string>();

            for (var planetIndex = 1; planetIndex < data.Count; planetIndex++)
            {
                var planet = data[planetIndex];

                if (!IncludePlanetInfoInList(
                        planet[(int)DataDescriptionIndexes.PlanetName],
                        planet[(int)DataDescriptionIndexes.PlanetType]))
                {
                    continue;
                }

                planetDescriptions.Add(planet[(int)DataDescriptionIndexes.PlanetDescription]);
            }

            return planetDescriptions;
        }

        private bool IncludePlanetInfoInList(string planetName, string planetType)
        {
            var isMoonOrDwarf = planetType.Equals(MoonDetector) || planetType.Equals(DwarfDetector);
            var isEarthsMoon = planetName.Equals(EarthsMoonName);
            var isSandboxMode = allowPropertyEditing;

            return !isMoonOrDwarf || (isEarthsMoon && isSandboxMode);
        }


        /// <summary>
        /// Adds a new item to the planet list based on the provided GameObject
        /// </summary>
        /// 
        /// <param name="planetObject">The GameObject representing the celestial body to be added</param>
        public void AddNewCelestialBody(GameObject planetObject)
        {
            Dictionary<string, TwoObjectContainer<Func<string>, UnityAction<string>>> variableProperties = new();

            if (allowPropertyEditing)
            {
                variableProperties = PlanetListDictionaries.GetVariablePropertiesDictionary(planetObject);
            }

            var liveStats = PlanetListDictionaries.GetLiveStatsDictionary(planetObject, sun);

            CreateNewPlanet(planetSprites[3], planetObject.name, planetObject, variableProperties, new Dictionary<string, string>(), liveStats, planetObject.name);
        }
    }
}