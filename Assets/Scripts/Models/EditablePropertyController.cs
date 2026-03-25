using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using SystemObject = System.Object;

namespace Models
{
    /// <summary>
    /// Controller Script for Prefab "Editable Property Prefab"
    /// </summary>
    public class EditablePropertyController : PropertyFieldController
    {
        [SerializeField] private TMP_InputField planetPropertyText;
        

        /// <summary>
        /// Adds a listener that will be called when a selection event occurs in any of the relevant Input Fields
        /// </summary>
        /// 
        /// <param name="listener">
        /// The UnityAction that will be invoked when the selection event occurs
        /// </param>
        public override void AddListenerToAllOnSelection(UnityAction<string> listener)
        {
            planetPropertyText.onSelect.AddListener(listener);
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
            planetPropertyText.onEndEdit.AddListener(listener);
        }

        /// <summary>
        /// Allows external scripts to edit the text inside the InputField by code
        /// </summary>
        /// 
        /// <param name="text">
        /// The new text that should be stored in the InputField
        /// </param>
        public override void SetText(SystemObject[] text)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var stringPiece in text)
            {
                stringBuilder.Append(stringPiece.ToString()); ;
            }

            planetPropertyText.text = stringBuilder.ToString();
        }

        /// <summary>
        /// Destroys this instance of the Prefab
        /// </summary>
        public void DestroyProperty()
        {
        #if UNITY_EDITOR
            // Use DestroyImmediate when in the Editor
            UnityEngine.Object.DestroyImmediate(gameObject);
        #else
            // Use Destroy when at runtime
            Destroy(gameObject);
        #endif
        }

    }
}