using UnityEngine;

namespace Kalkatos.DottedArrow
{
    public class TutorialArrow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform origin;
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform baseRect;

        [Header("Settings")]
        [SerializeField] private float baseHeight = 100f;
        [SerializeField] private bool startsActive = false;
        [SerializeField] private Vector3 originOffset = Vector3.zero; // NEW: Offset from player position

        private RectTransform myRect;
        private Canvas canvas;
        private Camera mainCamera;
        private bool isActive;

        private void Awake()
        {
            // FIXED: Use GetComponent instead of casting
            myRect = GetComponent<RectTransform>();
            
            if (myRect == null)
            {
                Debug.LogError("TutorialArrow must be a UI element with RectTransform! Add it to a Canvas.");
                return;
            }
            
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError(" TutorialArrow must be a child of a Canvas!");
                return;
            }
            
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No Main Camera found! Tag your camera as MainCamera.");
                return;
            }
            
            SetActive(startsActive);
        }

        private void Update()
        {
            if (!isActive || target == null || origin == null)
                return;

            Setup();
        }

        private void Setup()
        {
            if (myRect == null || canvas == null || mainCamera == null)
                return;

            // Convert world positions to screen positions
            Vector2 originPosOnScreen = mainCamera.WorldToScreenPoint(origin.position);
            Vector2 targetPosOnScreen = mainCamera.WorldToScreenPoint(target.position);

            // Place the arrow's base where the origin is
            myRect.anchoredPosition = (originPosOnScreen - new Vector2(Screen.width / 2, Screen.height / 2)) / canvas.scaleFactor;

            // Direction from origin to target
            Vector2 direction = targetPosOnScreen - originPosOnScreen;

            // Rotate arrow to point at target
            transform.up = direction;

            // Stretch dotted arrow based on distance
            if (baseRect != null && baseHeight > 0)
            {
                baseRect.anchorMax = new Vector2(baseRect.anchorMax.x, direction.magnitude / canvas.scaleFactor / baseHeight);
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
            if (baseRect != null)
            {
                baseRect.gameObject.SetActive(active);
            }
            if (active && origin != null && target != null)
            {
                Setup();
            }
        }

        public void SetTargets(Transform newOrigin, Transform newTarget)
        {
            origin = newOrigin;
            target = newTarget;
            SetActive(true);
        }

        public void Deactivate() => SetActive(false);
    }
}