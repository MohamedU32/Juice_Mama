using UnityEngine;

public class JuicePacketScript : MonoBehaviour
{
    [Header("Juice Packet Settings")]
    public AudioClip collectedSoundEffect;
    [HideInInspector] public JuiceData juiceData;
    [SerializeField] private float moveSpeed = 5f; // Speed to move toward the player

    private GameObject player;
    private bool moveToPlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
    }

    private void Update()
    {
        // 1️⃣ Move the packet toward the player if clicked
        if (moveToPlayer && player != null)
        {
            Vector3 targetPos = player.transform.position + Vector3.up * 1f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                // Add juice to fridge when packet reaches player
                if (juiceData != null)
                    JuiceFridgeManager.Instance.AddJuice(juiceData, 1); // Event will update UI automatically

                // Remove packet from scene
                gameObject.SetActive(false);
            }
        }

        // 2️⃣ Detect clicks on the packet using raycast
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    moveToPlayer = true;

                    // Play collection sound
                    var audioSource = player.GetComponent<PlayerController>()?.playerAudioSource;
                    if (audioSource != null && collectedSoundEffect != null)
                        audioSource.PlayOneShot(collectedSoundEffect, 1f);

                    Debug.Log("Juice packet clicked!");
                }
            }
        }
    }

    // Optional: automatically collect if player touches it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && juiceData != null)
        {
            JuiceFridgeManager.Instance.AddJuice(juiceData, 1); // Event updates UI
            gameObject.SetActive(false);
        }
    }
}
