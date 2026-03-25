using TMPro;
using UnityEngine;
using UnityEngine.Events;
using SystemObject = System.Object;


namespace Models
{
    /// <summary>
    /// Controller Script for Prefab "Observable Property Prefab"
    /// </summary>
    public class ObservablePropertyController : PropertyFieldController
    {
        [SerializeField] private TMP_InputField planetPropertyDescription;
        [SerializeField] private TMP_InputField planetPropertyValue;
        [SerializeField] private TMP_InputField planetPropertyMeasurementUnit;

        public bool IsBeingEdited { get; private set; }

        /// <summary>
        /// Adds a listener that will be called when an editing end event occurs in the Property Value Input Field
        /// </summary>
        /// 
        /// <param name="listener">
        /// The UnityAction that will be invoked when the event occurs
        /// </param>
        public void AddListenerToPropertyValueEditEnd(UnityAction<string> listener)
        {
            planetPropertyValue.onEndEdit.AddListener(listener);
        }

        /// <summary>
        /// Adds a listener that will be called when a selection event occurs in any of the relevant Input Fields
        /// </summary>
        /// 
        /// <param name="listener">
        /// The UnityAction that will be invoked when the selection event occurs
        /// </param>
        public override void AddListenerToAllOnSelection(UnityAction<string> listener)
        {
            planetPropertyDescription.onSelect.AddListener(listener);
            planetPropertyValue.onSelect.AddListener(listener);
            planetPropertyMeasurementUnit.onSelect.AddListener(listener);
        }

        /// <summary>
        /// Adds a listener that will be called when an editing end event occurs in any of the relevant Input Fields
        /// </summary>
        /// 
        /// <param name="listener">
        /// The UnityAction that will be invoked when the event occurs
        /// </param>
        public override void AddListenerToAllOnEditEnd(UnityAction<string> listener)
        {
            planetPropertyDescription.onEndEdit.AddListener(listener);
            planetPropertyValue.onEndEdit.AddListener(listener);
            planetPropertyMeasurementUnit.onEndEdit.AddListener(listener);
        }

        private void Start()
        {
            planetPropertyValue.onSelect.AddListener(_ => IsBeingEdited = true);
            AddListenerToPropertyValueEditEnd(_ => IsBeingEdited = false);
        }


        /// <summary>
        /// Allows external scripts to edit the text inside the InputField by code
        /// </summary>
        /// 
        /// <param name="text">
        /// The new values that should be stored in the InputField. Text is intended to be an array with three elements:
        /// - The name of the property
        /// - The value of the property
        /// - The measurement unit of the property
        /// </param>
        public override void SetText(SystemObject[] text)
        {
            planetPropertyDescription.text = text[(int)DataIndexes.PropertyDescription].ToString();

            SetValue(text[(int)DataIndexes.PropertyValue].ToString());
            SetUnit(text[(int)DataIndexes.PropertyUnit].ToString());
        }

        private void SetValue(string value)
        {
            planetPropertyValue.text = value;
        }
        private void SetUnit(string text)
        {
            text ??= "";

            planetPropertyMeasurementUnit.text = text;
        }
    }
}

