using UnityEngine;
using UnityEngine.UI;

public class TutorialsArrow : MonoBehaviour
{
    public GameObject player;
    public GameObject target;

    public float minScale = 0.5f;
    public float maxScale = 1.5f;
    public float maxDistance = 20f;
    public float offsetFromMidpoint = 2.5f;
    public float hideDistance = 1.5f; // Distance at which arrow disappears

    private RectTransform arrowUI;
    private Image arrowImage;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        arrowUI = gameObject.GetComponent<RectTransform>();
        arrowImage = gameObject.GetComponent<Image>();
    }

    void Update()
    {
        if (player == null || target == null || arrowUI == null || arrowImage == null) return;

        float worldDistance = Vector3.Distance(player.transform.position, target.transform.position);

        // Hide arrow if too close
        bool shouldShow = worldDistance > hideDistance;
        arrowUI.gameObject.SetActive(shouldShow);
        if (!shouldShow) return;

        // Convert world positions to screen space
        Vector3 playerScreen = Camera.main.WorldToScreenPoint(player.transform.position);
        Vector3 targetScreen = Camera.main.WorldToScreenPoint(target.transform.position);

        // 1. Position arrow halfway between player and goal
        Vector3 midPoint = (playerScreen + targetScreen) / 2f;
        // Clamp to screen bounds
        float padding = 50f; // Optional: keeps arrow away from edges
        midPoint.x = Mathf.Clamp(midPoint.x, padding, Screen.width - padding);
        midPoint.y = Mathf.Clamp(midPoint.y, padding, Screen.height - padding);
        arrowUI.position = midPoint + Vector3.up * offsetFromMidpoint;

        // 2. Rotate arrow to point toward goal
        Vector2 direction = targetScreen - playerScreen;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Scale arrow based on distance
        float scale = Mathf.Clamp(worldDistance / maxDistance, minScale, maxScale);
        arrowUI.localScale = new Vector3(scale, scale, 1);
    }
}
