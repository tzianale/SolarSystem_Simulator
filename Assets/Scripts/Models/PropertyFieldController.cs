using UnityEngine;
using UnityEngine.Events;
using SystemObject = System.Object;

namespace Models
{
    /// <summary>
    /// Abstract class for Prefabs that are willing to display properties into the planet info tab
    /// </summary>
    public abstract class PropertyFieldController : MonoBehaviour
    {
        protected enum DataIndexes
        {
            PropertyDescription,
            PropertyValue,
            PropertyUnit
        }

        /// <summary>
        /// Erases the old text that was being displayed and updates it to the newly given value
        /// </summary>
        /// 
        /// <param name="text">
        /// The updated Text
        /// </param>
        public abstract void SetText(SystemObject[] text);

        public abstract void AddListenerToAllOnSelection(UnityAction<string> listener);
        public abstract void AddListenerToAllOnEditEnd(UnityAction<string> listener);
    }
}