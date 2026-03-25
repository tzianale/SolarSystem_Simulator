using Utils;
using Models;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Controller for the Prefabs that will be used as Elements in the Planet List
    /// </summary>
    public class PlanetListElementPrefabController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject planetSprite;

        [SerializeField]
        private GameObject planetName;

        [SerializeField]
        private CameraControlV2 cameraControl;
        

        private GameObject _planetInfoTab;
        private GameObject _planet3DObject;
        
        private Wrapper<GameObject> _currentlyActiveTab;
        private Wrapper<GameObject> _currentlyLightedPlanet;

        /// <summary>
        /// Constructor-like method, sets all the relevant information and references, as well as linking the Closing Button
        /// to the Close Tab method, allowing the script to work correctly
        /// </summary>
        /// 
        /// <param name="inputSprite">
        /// Reference to the element where the planet icon will be placed
        /// </param>
        /// 
        /// <param name="inputName">
        /// The name of the planet
        /// </param>
        /// 
        /// <param name="planetModel">
        /// Reference to the Sphere Object in the simulation which is representing this planet
        /// </param>
        /// 
        /// <param name="cameraCtrl">
        /// Reference to the Camera Controller Script for this simulation
        /// </param>
        /// 
        /// <param name="linkedInfoTab">
        /// The info tab with the properties of this planet
        /// </param>
        /// 
        /// <param name="referenceToActiveTab">
        /// Wrapper object containing continuously updated info about the info tab that is currently open
        /// </param>
        /// 
        /// <param name="referenceToHighlightedPlanet">
        /// Wrapper object containing continuously updated info about the planet that is currently being focused on
        /// </param>
        /// 
        /// <param name="linkedCloseButton">
        /// Reference to the button that, when pressed, should close the info tab
        /// </param>
        public void SetPlanetInfo(Sprite inputSprite, string inputName, GameObject planetModel, CameraControlV2 cameraCtrl, 
            GameObject linkedInfoTab, Wrapper<GameObject> referenceToActiveTab, 
            Wrapper<GameObject> referenceToHighlightedPlanet, Button linkedCloseButton)
        {
            planetSprite.GetComponent<Image>().sprite = inputSprite;
            planetName.GetComponent<TextMeshProUGUI>().text = inputName;

            _planetInfoTab = linkedInfoTab;
            _planet3DObject = planetModel;

            cameraControl = cameraCtrl;
            _currentlyActiveTab = referenceToActiveTab;
            _currentlyLightedPlanet = referenceToHighlightedPlanet;

            linkedCloseButton.onClick.AddListener(CloseThisTab);
        }

        /// <summary>
        /// Registers clicks to the Planet List Element.
        /// The number of clicks is then given to the HandleClickEvent function
        /// </summary>
        /// 
        /// <param name="eventData">
        /// The object storing various information about the click event
        /// </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            HandleClickEvent(eventData.clickCount);
        }

        /// <summary>
        /// Handles the events following a click on a planet:
        /// One click should open the info tab,
        /// Two clicks should tell the camera to focus on the 3D object corresponding to this specific planet
        /// </summary>
        /// 
        /// <param name="clickCount">
        /// How many times the button was clicked
        /// </param>
        public void HandleClickEvent(int clickCount)
        {
            switch (clickCount)
            {
                case 1:
                    if(CloseCurrentlyOpenTab()) break;
                    
                    _planetInfoTab.SetActive(true);
                    
                    _currentlyActiveTab.SetValue(_planetInfoTab);
                    _currentlyLightedPlanet.SetValue(_planet3DObject);
                    
                    break;
                case 2: 
                    if (cameraControl.GetFollowingTarget() != null && cameraControl.GetFollowingTarget().Equals(_planet3DObject.transform))
                    {
                        cameraControl.StopFollowing();
                    }
                    else
                    {
                        cameraControl.FollowObject(_planet3DObject.transform);
                    }
                    break;
            }
        }

        private void CloseThisTab()
        {
            if (_currentlyActiveTab.GetValue() == _planetInfoTab)
            {
                CloseTab(_planetInfoTab);
            } 
        }

        private void CloseTab(GameObject tabToClose)
        {
            tabToClose.SetActive(false);

            if (_currentlyActiveTab.GetValue() == tabToClose)
            {
                _currentlyActiveTab.SetValue(null);
                _currentlyLightedPlanet.SetValue(null);
            }
        }

        private bool CloseCurrentlyOpenTab()
        {
            if (_currentlyActiveTab.GetValue() == _planetInfoTab)
            {
                CloseTab(_planetInfoTab);
                return true;
            } 
            
            if (_currentlyActiveTab.GetValue() != null)
            {
                CloseTab(_currentlyActiveTab.GetValue());
            }

            return false;
        }
    }
}