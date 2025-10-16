using UnityEngine;

//for 3d asset arrow(incase we switch assets)
public class ArrowController : MonoBehaviour
{
    public Transform player;
    private Transform target;

    [Header("Arrow Settings")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;
    public float maxDistance = 15f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null) return;

        // Rotate arrow toward target
        Vector3 direction = target.position - player.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);

        // Follow player position
        transform.position = player.position + Vector3.up * 0.1f;

        // Scale arrow depending on distance
        float distance = Vector3.Distance(player.position, target.position);
        float t = Mathf.Clamp01(distance / maxDistance);
        float scale = Mathf.Lerp(maxScale, minScale, t);
        transform.localScale = Vector3.one * scale;
    }
}
