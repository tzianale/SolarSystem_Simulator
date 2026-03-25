using TMPro;
using UnityEngine;
using System.Collections;

namespace Models
{
    /// <summary>
    /// Handles the Planet List animation on button click
    /// </summary>
    public class PlanetListAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform planetListContainerTransform;
        [SerializeField] private RectTransform planetListScrollViewTransform;

        [SerializeField] private Vector3 moveOffset;
        [SerializeField] private float moveSpeed = 1.0f;

        private bool _listIsOpen;
        private bool _initialised;

        public TextMeshProUGUI buttonText;
        private Vector3 _moveDistanceImproved;


        public RectTransform PlanetListContainerTransform { private get; set; }

        private Vector3 MoveOffset { get; set; }

        private float MoveSpeed { get; set; }

        public bool ListIsOpen { get; private set; }


        private void Start()
        {
            PlanetListContainerTransform = planetListContainerTransform;
            MoveOffset = moveOffset;
            MoveSpeed = moveSpeed;
            ListIsOpen = _listIsOpen;
        }

        private void Update()
        {
            if (PlanetListContainerTransform)
            {
                _moveDistanceImproved = MoveOffset;
                _moveDistanceImproved.y += planetListScrollViewTransform.rect.height
                                           * planetListScrollViewTransform.lossyScale.y;
            }
        }

        /// <summary>
        /// Called on button click, starts a new Coroutine where the list will be moved to the new position
        /// </summary>
        public void OnArrowClick()
        {
            if (ListIsOpen)
            {
                StartCoroutine(MovePanel(-_moveDistanceImproved));
                ListIsOpen = false;
                buttonText.text = "↑";
            }
            else
            {
                StartCoroutine(MovePanel(_moveDistanceImproved));
                ListIsOpen = true;
                buttonText.text = "↓";
            }
        }

        private IEnumerator MovePanel(Vector3 vectorToTarget)
        {
            var startPosition = PlanetListContainerTransform.position;
            var endPosition = startPosition + vectorToTarget;

            var currentTime = 0.0f;

            while (currentTime < 1)
            {
                currentTime += Time.deltaTime * MoveSpeed;
                PlanetListContainerTransform.position = Vector3.Lerp(startPosition, endPosition, currentTime);
                yield return null;
            }

            PlanetListContainerTransform.position = endPosition;
        }
    }
}