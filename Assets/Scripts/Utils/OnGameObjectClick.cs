using System;
using UnityEngine;
using System.Collections.Generic;


namespace Utils
{
    /// <summary>
    /// OnGameObjectClick provides a framework for reacting to clicks on GameObjects.
    /// The Script also supports double / triple / quadruple / etc. click detection
    /// </summary>
    public class OnGameObjectClick : MonoBehaviour
    {
        private const int LeftMouseButton = 0;
        private const int CounterStepForClickCounts = 1;
        private const int CounterStartForClickCounts = 1;
        private const float MaxTimeBetweenMultipleClicks = 1f;
        
        private Camera _gameCamera;
        
        private int _lastClickCount;
        private float _lastClickTime;
        
        private List<Action<int>> _onClickActions;

        private void Awake()
        {
            _gameCamera = Camera.main;
            _lastClickTime = Time.time;
        }

        /// <summary>
        /// Sets the actions that will be executed on click
        /// </summary>
        /// 
        /// <param name="actionsInitializer">
        /// The Action list. Actions return void and take an integer (click count) as parameter
        /// </param>
        public void SetActions(List<Action<int>> actionsInitializer)
        {
            _onClickActions = actionsInitializer;
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(LeftMouseButton))
            {
                var mousePosition = Input.mousePosition;
                var ray = _gameCamera.ScreenPointToRay(mousePosition);
                
                if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
                {
                    ExecuteClickActions(GetClickCount());
                }
            }
        }

        private void ExecuteClickActions(int clickCount)
        {
            foreach (var action in _onClickActions)
            {
                action.Invoke(clickCount);
            }
        }

        private int GetClickCount()
        {
            var currentTime = Time.time;
            var timeBetweenClicks = currentTime - _lastClickTime;

            if (timeBetweenClicks <= MaxTimeBetweenMultipleClicks)
            {
                _lastClickCount += CounterStepForClickCounts;
            }
            else
            {
                _lastClickCount = CounterStartForClickCounts;
            }

            _lastClickTime = currentTime;
            return _lastClickCount;
        }
    }
}