using UnityEngine;

namespace Kalkatos.DottedArrow
{
    [RequireComponent(typeof(RectTransform))]
    public class TutorialArrow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform origin;
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform baseRect; // The dotted line or arrow body

        [Header("Settings")]
        [SerializeField] private float baseHeight = 100f;
        [SerializeField] private bool startsActive = false;
        [SerializeField] private Vector3 originOffset = Vector3.zero; // Offset from origin position

        private RectTransform myRect;
        private Canvas canvas;
        private Camera mainCamera;
        private bool isActive;

        private void Awake()
        {
            myRect = GetComponent<RectTransform>();

            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("❌ TutorialArrow must be a child of a Canvas!");
                enabled = false;
                return;
            }

            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("❌ No Main Camera found! Tag your camera as 'MainCamera'.");
                enabled = false;
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
            // Convert world positions to screen space
            Vector3 originPosOnScreen = mainCamera.WorldToScreenPoint(origin.position + originOffset);
            Vector3 targetPosOnScreen = mainCamera.WorldToScreenPoint(target.position);

            // Convert to canvas space (so arrow aligns correctly)
            Vector2 canvasOrigin;
            Vector2 canvasTarget;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, originPosOnScreen, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out canvasOrigin);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, targetPosOnScreen, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out canvasTarget);

            // Set position of the arrow base
            myRect.anchoredPosition = canvasOrigin;

            // Direction vector in canvas space
            Vector2 direction = canvasTarget - canvasOrigin;

            // Rotate arrow to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            myRect.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 aligns 'up' with direction

            // Stretch the dotted arrow based on distance
            if (baseRect != null)
            {
                float distance = direction.magnitude;
                Vector2 newSize = baseRect.sizeDelta;
                newSize.y = distance / (canvas.scaleFactor);
                baseRect.sizeDelta = newSize;
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
            gameObject.SetActive(active);

            if (active && origin != null && target != null)
                Setup();
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
