using System;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// A generic wrapper class that holds a value and allows registering actions to be performed
    /// when the value is changed
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value to be wrapped</typeparam>
    public class Wrapper<T>
    {
        private T _savedValue;
        private readonly List<Action<T, T>> _onSetValue;
        
        /// <summary>
        /// Initializes a new instance of the class with a specified initial value
        /// </summary>
        /// 
        /// <param name="elementInitializer">The initial value to be wrapped</param>
        public Wrapper(T  elementInitializer)
        {
            _onSetValue = new List<Action<T, T>>();
            _savedValue = elementInitializer;
        }

        /// <summary>
        /// Sets a new value and invokes all registered actions with the old and new values
        /// </summary>
        /// 
        /// <param name="newValue">The new value to be set</param>
        public void SetValue(T newValue)
        {
            foreach (var action in _onSetValue)
            {
                action.Invoke(_savedValue, newValue);
            }
            
            _savedValue = newValue;
        }

        /// <summary>
        /// Gets the current value
        /// </summary>
        /// 
        /// <returns>The current value of the wrapped element</returns>
        public T GetValue()
        {
            return _savedValue;
        }

        /// <summary>
        /// Adds an action to be invoked when the value is set.
        /// The action receives the old value and the new value as parameters
        /// </summary>
        /// 
        /// <param name="action">The action to be invoked when the value is set</param>
        public void AddOnSetValueAction(Action<T, T> action)
        {
            _onSetValue.Add(action);
        }
    }
}