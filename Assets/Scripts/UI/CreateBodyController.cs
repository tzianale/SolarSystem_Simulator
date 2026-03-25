using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Image = UnityEngine.UI.Image;

namespace UI
{
    /// <summary>
    /// Manages the UI for creating a new celestial body, including input validation
    /// and interaction with the camera control and planet list manager
    /// </summary>
    public class CreateBodyController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputFieldName;
        
        [SerializeField] private TMP_Dropdown inputFieldType;
        
        [SerializeField] private TMP_InputField inputFieldMass;
        [SerializeField] private TMP_InputField inputFieldDiameter;
        
        [SerializeField] private TMP_InputField inputFieldPositionX;
        [SerializeField] private TMP_InputField inputFieldPositionY;
        [SerializeField] private TMP_InputField inputFieldPositionZ;
        
        [SerializeField] private TMP_InputField inputFieldInitialVelocityX;
        [SerializeField] private TMP_InputField inputFieldInitialVelocityY;
        [SerializeField] private TMP_InputField inputFieldInitialVelocityZ;
        
        [SerializeField] private Image colorPickerButtonImage;
        [SerializeField] private GameObject colorPickerPanel;
        
        [SerializeField] private PlanetListManager planetListManager;
        
        [SerializeField] private GameObject createBodyPanel;
        
        [SerializeField] private CameraControlV2 cameraControl;

        private Color _selectedColor = Color.blue;
        private readonly Color _errorColor = new(1f, 0.49f, 0.49f);

        private void Start()
        {
            List<TMP_InputField> inputFields = new()
            {
                inputFieldName,
                inputFieldMass,
                inputFieldDiameter,
                inputFieldPositionX,
                inputFieldPositionY,
                inputFieldPositionZ,
                inputFieldInitialVelocityX,
                inputFieldInitialVelocityY,
                inputFieldInitialVelocityZ
            };
            
            UnityAction<string> onFieldSelected = _ => cameraControl.SetKeyboardLock(true);
            UnityAction<string> onFieldDeselected = _ => cameraControl.SetKeyboardLock(false);

            foreach (var inputField in inputFields)
            {
                AssignListenersToInputField(inputField, onFieldSelected, onFieldDeselected);
            }
        }

        private void AssignListenersToInputField(TMP_InputField inputField, 
            UnityAction<string> onEditStart,
            UnityAction<string> onEditEnd)
        {
            inputField.onSelect.AddListener(onEditStart);
            inputField.onEndEdit.AddListener(onEditEnd);
        }

        /// <summary>
        /// Creates a new celestial body for the simulation (depending on the input fields) and
        /// adds the generated GameObject to the planet list
        /// </summary>
        public void CreateNewCelestialBody()
        {
            if (!ValidateInputFields()) return;
            GameObject newCelestialBody = CelestialBodyGenerator.CreateNewCelestialBodyGameObject(
                inputFieldName.text,
                (CelestialBodyType)inputFieldType.value,
                new Vector3(float.Parse(inputFieldPositionX.text), float.Parse(inputFieldPositionY.text), float.Parse(inputFieldPositionZ.text)),
                float.Parse(inputFieldMass.text),
                float.Parse(inputFieldDiameter.text),
                new Vector3(float.Parse(inputFieldInitialVelocityX.text), float.Parse(inputFieldInitialVelocityY.text), float.Parse(inputFieldInitialVelocityZ.text)),
                _selectedColor
            );

            planetListManager.AddNewCelestialBody(newCelestialBody);
            createBodyPanel.SetActive(false);
        }

        /// <summary>
        /// Changes the currently selected color to a new value
        /// </summary>
        /// 
        /// <param name="color">
        /// The new currently selected color
        /// </param>
        public void OnSelectedColorChange(Color color)
        {
            colorPickerButtonImage.color = color;
            _selectedColor = color;
        }

        private bool ValidateInputFields()
        {
            bool isValid = true;
            ValidateField(inputFieldName, ref isValid);
            ValidatePositiveNumber(inputFieldMass, ref isValid);
            ValidatePositiveNumber(inputFieldDiameter, ref isValid);
            ValidateField(inputFieldPositionX, ref isValid);
            ValidateField(inputFieldPositionY, ref isValid);
            ValidateField(inputFieldPositionZ, ref isValid);
            ValidateField(inputFieldInitialVelocityX, ref isValid);
            ValidateField(inputFieldInitialVelocityY, ref isValid);
            ValidateField(inputFieldInitialVelocityZ, ref isValid);

            return isValid;
        }

        private void ValidateField(TMP_InputField field, ref bool isValid)
        {
            if (!string.IsNullOrWhiteSpace(field.text)) return;
            field.image.color = _errorColor;
            isValid = false;
        }

        private void ValidatePositiveNumber(TMP_InputField field, ref bool isValid)
        {
            ValidateField(field, ref isValid);
            if (float.TryParse(field.text, out float value) && value > 0) return;
            field.image.color = _errorColor;
            isValid = false;
        }

        /// <summary>
        /// Resets the image of an Input Field to white
        /// </summary>
        /// 
        /// <param name="field">
        /// The input field whose color has to be reset
        /// </param>
        public void ResetInputColor(TMP_InputField field)
        {
            field.image.color = Color.white;
        }
    }
}
