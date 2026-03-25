using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


namespace Models
{
    /// <summary>
    /// Second iteration of teh Camera Control Script,
    /// handles camera control functionalities such as panning, orbiting, zooming, and following targets
    /// </summary>
    public class CameraControlV2 : MonoBehaviour
    {
        [SerializeField]
        private GameObject sun;

        [SerializeField]
        private Camera cam;

        [SerializeField]
        private GameObject camHolder;

        [SerializeField]
        private GameObject neptune;

        [SerializeField]
        private float minDistanceToSurfaceRadiusFactor;

        // Constants for mouse buttons
        private const int LeftMouseButton = 0;
        private const int RightMouseButton = 1;

        private const float NoMouseScroll = 0f;
        private const float ScrollTolerance = 0.001f;

        private const float PositionZero = 0;
        private const float PositionTolerance = 0.00001f;

        private Vector3 _pivotPoint;

        private bool _followModeActive;
        private Transform _followTarget;

        private float _camToPivotDistance;
        private Vector3 _camToPivotDirection;


        // Defaults
        private float _defaultCamToPivotDistance;
        private Vector3 _defaultCamToPivotDirection;
        private Quaternion _defaultRotation;


        // Zoom stuff
        private float _zoomStrength = 1f;
        private float _maxZoomDistance;

        private const float MinZoomDistance = 10.0f;
        private const float MaxZoomDistanceMultiplier = 2;

        // Rotation stuff
        private Vector3 _lastMousePosition;
        private const float RotationSpeed = 1f;

        // Variables for panning
        private Vector3 _initialMousePosition;
        private Vector3 _initialPivotPoint;
        private bool _panMode;

        // Keymappings
        private Dictionary<KeyCode, Action> _keyMappings;

        private static readonly Vector3 Keypad7FixedView = new(1, 1, 1);
        private static readonly Vector3 Alpha7FixedView = new(1, 1, 1);

        private bool _keyboardLock;


        private void Start()
        {
            InitializeKeyMappings();

            if (neptune != null)
                _maxZoomDistance = neptune.transform.position.magnitude * MaxZoomDistanceMultiplier;
            else
                _maxZoomDistance = 10000f;

            _pivotPoint = sun.transform.position;

            _defaultRotation = camHolder.transform.rotation;
            var camToPivotCombined = camHolder.transform.position - _pivotPoint;
            _camToPivotDirection = camToPivotCombined.normalized;
            _defaultCamToPivotDirection = _camToPivotDirection;
            _camToPivotDistance = camToPivotCombined.magnitude;
            _defaultCamToPivotDistance = _camToPivotDistance;
        }


        private void Update()
        {
            if (_followModeActive)
            {
                FollowTarget();
            }

            else
            {
                HandlePan();
            }

            if (!_keyboardLock)
            {
                HandleDefaultMovement();
            }

            UpdateCameraPosition();
        }


        private void InitializeKeyMappings()
        {
            _keyMappings = new Dictionary<KeyCode, Action>
            {
                { KeyCode.Keypad1, () => SetFixedView(Vector3.up) }, // np1
                { KeyCode.Alpha1,  () => SetFixedView(Vector3.up) }, // kb1
                { KeyCode.Keypad3, () => SetFixedView(Vector3.forward) },
                { KeyCode.Alpha3,  () => SetFixedView(Vector3.forward) },

                { KeyCode.Keypad7, () => SetFixedView(Keypad7FixedView) },
                { KeyCode.Alpha7,  () => SetFixedView(Alpha7FixedView) },

                { KeyCode.R,       ResetCameraView },
                { KeyCode.Keypad6, RotateCam45DegRight },
                { KeyCode.Alpha6,  RotateCam45DegRight },
                { KeyCode.Keypad4, RotateCam45DegLeft },
                { KeyCode.Alpha4,  RotateCam45DegLeft },
                { KeyCode.Keypad8, RotateCam45DegUp },
                { KeyCode.Alpha8,  RotateCam45DegUp },
                { KeyCode.Keypad2, RotateCam45DegDown },
                { KeyCode.Alpha2,  RotateCam45DegDown }
            };
        }

        private void CheckForKeyPresses()
        {
            foreach (var keyMapping in _keyMappings.Where(keyValuePair => Input.GetKeyDown(keyValuePair.Key)))
            {
                keyMapping.Value.Invoke();
            }
        }

        private void FollowTarget()
        {
            _pivotPoint = _followTarget.position;
        }

        private void HandlePan()
        {
            if (Input.GetMouseButtonDown(LeftMouseButton))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    _initialMousePosition = Input.mousePosition;
                    _initialPivotPoint = _pivotPoint;
                    _panMode = true;
                }
                else
                {
                    _panMode = false;
                }
            }

            if (Input.GetMouseButton(LeftMouseButton) && _panMode)
            {
                Vector3 currentMousePosition = Input.mousePosition;

                Ray ray = cam.ScreenPointToRay(_initialMousePosition);
                Plane plane = new Plane(cam.transform.forward, _initialPivotPoint);

                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    ray = cam.ScreenPointToRay(currentMousePosition);
                    plane.Raycast(ray, out distance);
                    Vector3 currentHitPoint = ray.GetPoint(distance);
                    Vector3 panDelta = hitPoint - currentHitPoint;

                    _pivotPoint = _initialPivotPoint + panDelta;
                }
            }
        }

        private void HandleDefaultMovement()
        {
            if (Input.GetMouseButtonDown(RightMouseButton))
            {
                UpdateLastMousePosition();
            }

            if (Input.GetMouseButton(RightMouseButton))
            {
                CamOrbit();
            }

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                var mouseScroll = Input.mouseScrollDelta.y;

                if (!FloatEqualToValue(mouseScroll, NoMouseScroll, ScrollTolerance))
                {
                    Zoom(mouseScroll);
                }
            }

            CheckForKeyPresses();
        }

        private void UpdateLastMousePosition()
        {
            _lastMousePosition = Input.mousePosition;
        }

        private void CamOrbit(float customDeltaX = 0, float customDeltaY = 0)
        {
            Vector3 delta = Input.mousePosition - _lastMousePosition;

            _lastMousePosition = Input.mousePosition;

            float horizontalInput = delta.x * RotationSpeed * 0.1f;
            float verticalInput = delta.y * RotationSpeed * 0.1f;

            if (!FloatEqualToValue(customDeltaX, PositionZero, PositionTolerance)
                ||
                !FloatEqualToValue(customDeltaY, PositionZero, PositionTolerance))
            {
                horizontalInput = customDeltaX;
                verticalInput = customDeltaY;
            }

            Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInput, Vector3.up);
            Quaternion verticalRotation = Quaternion.AngleAxis(-verticalInput, camHolder.transform.right);

            _camToPivotDirection = verticalRotation * horizontalRotation * _camToPivotDirection;

            _camToPivotDirection = _camToPivotDirection.normalized;

            camHolder.transform.rotation = horizontalRotation * verticalRotation * camHolder.transform.rotation;
        }

        private void RotateCam45DegRight()
        {
            CamOrbit(-45f);
        }

        private void RotateCam45DegLeft()
        {
            CamOrbit(45f);
        }

        private void RotateCam45DegUp()
        {
            CamOrbit(0f, -45f);
        }

        private void RotateCam45DegDown()
        {
            CamOrbit(0f, 45f);
        }

        private void Zoom(float mouseWheelStep)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _zoomStrength = 3f;
            }
            else
            {
                _zoomStrength = 1f;
            }
            _camToPivotDistance += -mouseWheelStep * CalculateDynamicZoomStep(_camToPivotDistance) * 2 * _zoomStrength;
        }

        private void ResetCameraView()
        {
            _followModeActive = false;
            _camToPivotDirection = _defaultCamToPivotDirection;
            _camToPivotDistance = _defaultCamToPivotDistance;
            camHolder.transform.rotation = _defaultRotation;
            _pivotPoint = sun.transform.position;
            _followTarget = null;
        }

        /// <summary>
        /// Stops the camera from following any targets
        /// </summary>
        public void StopFollowing()
        {
            _followModeActive = false;

        }

        /// <summary>
        /// Gets the target that the camera is currently following
        /// </summary>
        /// 
        /// <returns>
        /// The transform of the target being followed
        /// </returns>
        public Transform GetFollowingTarget()
        {
            return _followTarget;
        }

        /// <summary>
        /// Sets a specific target for the camera to follow 
        /// </summary>
        /// 
        /// <param name="target">
        /// The transform of the target to follow
        /// </param>
        public void FollowObject(Transform target)
        {
            if (target == null) return;
            _followModeActive = true;
            _followTarget = target;
            _camToPivotDistance = target.transform.lossyScale.x * 3;

        }


        private void SetFixedView(Vector3 direction)
        {
            _camToPivotDirection = direction;
            camHolder.transform.rotation = Quaternion.LookRotation(-direction);
        }

        private void UpdateCameraPosition()
        {
            var minZoomDistance = RayCastPlanetRadius();
            _camToPivotDistance = Mathf.Clamp(_camToPivotDistance, minZoomDistance, _maxZoomDistance);
            var camToPivotVector = _camToPivotDirection * _camToPivotDistance;
            camHolder.transform.position = _pivotPoint + camToPivotVector;
        }

        private float RayCastPlanetRadius()
        {
            Ray ray = new Ray(camHolder.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                float planetRadius = hit.transform.lossyScale.x / 2f;
                return planetRadius * minDistanceToSurfaceRadiusFactor;
            }

            return MinZoomDistance;
        }

        private static float CalculateDynamicZoomStep(float distanceCamPivot)
        {
            var step = Mathf.Pow(0.0003f * (distanceCamPivot + 10900), 3) - 34;
            return Mathf.Clamp(step, 1.0f, 300.0f);
        }

        /// <summary>
        /// Gets the current zoom scale as a value between 0 and 1, where 0 is the MinZoomDistance
        /// and 1 is the _maxZoomDistance
        /// </summary>
        /// 
        /// <returns>
        /// The zoom scale
        /// </returns>
        public float GetZoomScale()
        {
            return Mathf.InverseLerp(MinZoomDistance, _maxZoomDistance, _camToPivotDistance);
        }

        /// <summary>
        /// Sets the keyboard lock state.
        /// Lock state true prevents any keyboard input (useful for when text is being typed), while
        /// Lock state false allows it (normal conditions)
        /// </summary>
        /// 
        /// <param name="state">
        /// The lock state to set
        /// </param>
        public void SetKeyboardLock(bool state)
        {
            _keyboardLock = state;
        }

        private static bool FloatEqualToValue(float floatToCompare, float targetValue, float tolerance)
        {
            return Math.Abs(floatToCompare - targetValue) < tolerance;
        }

        public void SetSun(GameObject sun)
        {
            this.sun = sun;
        }

        public void SetCamera(Camera cam)
        {
            this.cam = cam;
        }

        public void SetCamHolder(GameObject camHolder)
        {
            this.camHolder = camHolder;
        }

        public void SetNeptune(GameObject neptune)
        {
            this.neptune = neptune;
        }

        public void SetFollowTarget(Transform target)
        {
            _followModeActive = true;
            _followTarget = target;
        }

    }
}