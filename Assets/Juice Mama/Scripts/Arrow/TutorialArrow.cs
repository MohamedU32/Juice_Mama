using UnityEngine;

public class TutorialArrow3D : MonoBehaviour
{
    [Header("Arrow Settings")]
    public Transform player;
    public float heightOffset = 2.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.5f;
    public float maxDistance = 15f;
    public float hideDistance = 2f;
    public float rotationSpeed = 10f;
    public float bobSpeed = 2f; // Bouncing animation speed
    public float bobAmount = 0.3f; // How much the arrow bobs up and down

    private Transform target;
    private Vector3 baseOffset;
    private bool isVisible = false;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        baseOffset = Vector3.up * heightOffset;
    }

    private void Update()
    {
        if (target == null || player == null)
        {
            if (isVisible) Hide();
            return;
        }

        float distance = Vector3.Distance(player.position, target.position);
        
        // Hide arrow when very close to target
        if (distance < hideDistance)
        {
            if (isVisible) Hide();
            return;
        }

        if (!isVisible) Show();
        UpdateArrow(distance);
    }

    private void UpdateArrow(float distance)
    {
        // Position arrow above player with bobbing animation
        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        Vector3 finalOffset = baseOffset + Vector3.up * bobOffset;
        transform.position = player.position + finalOffset;

        // Point arrow toward target (only horizontal rotation)
        Vector3 directionToTarget = target.position - player.position;
        directionToTarget.y = 0; // Keep arrow level

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Scale based on distance (closer = smaller, further = larger)
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float scale = Mathf.Lerp(maxScale, minScale, normalizedDistance);
        transform.localScale = Vector3.one * scale;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (newTarget != null)
        {
            Debug.Log($" Arrow target set to: {newTarget.name}");
            Show();
        }
        else
        {
            Debug.Log(" Arrow target cleared");
            Hide();
        }
    }

    public void Hide()
    {
        if (isVisible)
        {
            gameObject.SetActive(false);
            isVisible = false;
            Debug.Log(" Arrow hidden");
        }
    }

    public void Show()
    {
        if (!isVisible)
        {
            gameObject.SetActive(true);
            isVisible = true;
            Debug.Log("Arrow shown");
        }
    }

    // Optional: Draw debug line in Scene view
    private void OnDrawGizmos()
    {
        if (target != null && player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(player.position + Vector3.up * heightOffset, target.position);
            Gizmos.DrawWireSphere(target.position, hideDistance);
        }
    }
}