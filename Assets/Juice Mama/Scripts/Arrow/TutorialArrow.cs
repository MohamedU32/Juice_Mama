
using UnityEngine;
using UnityEngine.UI;

namespace Kalkatos.DottedArrow
{
    
    
    [RequireComponent(typeof(RectTransform))]
    public class TutorialArrow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Transform target;
        [SerializeField] private RectTransform baseRect; // Dotted line sprite
        [SerializeField] private Canvas canvas;

        [Header("Settings")]
        [SerializeField] private float maxDistance = 15f;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 1.5f;
        [SerializeField] private float hideDistance = 2f; // Hide arrow when player is close
        [SerializeField] private float rotationOffset = -90f; // Adjust if sprite points wrong
        [SerializeField] private bool startsActive = false;

        [Header("Next Target Settings")]
        [SerializeField] private Transform nextTarget; // Optional next target
        [SerializeField] private float nextTargetDelay = 1f;

        private RectTransform myRect;
        private Camera mainCamera;
        private CanvasGroup canvasGroup;
        private bool isActive;
        private bool taskCompleted;

        private void Awake()
        {
            myRect = GetComponent<RectTransform>();
            mainCamera = Camera.main;

            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                Debug.LogError(" TutorialArrow must be a child of a Canvas!");
                enabled = false;
                return;
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            SetActive(startsActive);
        }

        private void Update()
        {
            if (!isActive || player == null || target == null || taskCompleted)
                return;

            float distance = Vector3.Distance(player.position, target.position);
            if (distance < hideDistance)
            {
                canvasGroup.alpha = 0;
                return;
            }
            else
            {
                canvasGroup.alpha = 1;
            }

            UpdateArrow();
        }

        private void UpdateArrow()
        {
            // Convert world positions to screen points
            Vector2 playerScreen = RectTransformUtility.WorldToScreenPoint(mainCamera, player.position);
            Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(mainCamera, target.position);

            // Convert to canvas-local positions
            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, playerScreen, mainCamera, out Vector2 localPlayer);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, targetScreen, mainCamera, out Vector2 localTarget);

            // Position arrow at player
            myRect.anchoredPosition = localPlayer;

            // Rotate arrow
            Vector2 direction = localTarget - localPlayer;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            myRect.localRotation = Quaternion.Euler(0, 0, angle);

            // Stretch the dotted line
            if (baseRect != null)
            {
                float lineLength = direction.magnitude;
                Vector2 size = baseRect.sizeDelta;
                size.x = Mathf.Max(10f, lineLength); // Prevent zero-length
                baseRect.sizeDelta = size;
            }

            // Scale arrow based on distance
            float t = Mathf.Clamp01(Vector3.Distance(player.position, target.position) / maxDistance);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            myRect.localScale = Vector3.one * scale;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !taskCompleted)
                CompleteTask();
        }

        private void CompleteTask()
        {
            taskCompleted = true;
            SetActive(false);

            Debug.Log("Task Completed!");

            if (nextTarget != null)
                Invoke(nameof(ShowNextTarget), nextTargetDelay);
        }

        private void ShowNextTarget()
        {
            target = nextTarget;
            taskCompleted = false;
            SetActive(true);
        }

        public void SetTargets(Transform newPlayer, Transform newTarget)
        {
            player = newPlayer;
            target = newTarget;
            taskCompleted = false;
            SetActive(true);
        }

        public void SetActive(bool active)
        {
            isActive = active;
            gameObject.SetActive(active);
            if (active && canvasGroup != null)
                canvasGroup.alpha = 1;

            if (active)
                UpdateArrow();
        }

        public void Deactivate() => SetActive(false);
    }
}
