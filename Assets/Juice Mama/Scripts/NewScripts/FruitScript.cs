using UnityEngine;

public class FruitScript : MonoBehaviour
{
    [Header ("Fruit Growing Settings")]
    public bool isGrown = false;
    [SerializeField] private Vector3 startScale = new Vector3(0.25f, 0.25f, 0.25f);
    [SerializeField] private Vector3 targetScale = new Vector3(4, 4, 4);
    [SerializeField] private float scaleValue = 0.005f;

    [Header ("Fruit Settings")]

    [SerializeField] private float movespeed = 10.0f;  // Speed to move toward the player

    private GameObject player;
    private bool moveToPlayer = false;

    private void Start()
    {
        transform.localScale = startScale;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
    }

    // Update is called once per frame
    private void Update()
    {
        // Growing the fruit
        if (!isGrown)
        { 
            transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
            // Stop fruit growing when fruit reaches the target size
            if (transform.localScale.x >= targetScale.x) isGrown = true;
        }

        DetectFruitTouch();

        // Move the packet toward the player if clicked/tapped (collect Fruit)
        if (moveToPlayer && player != null)
        {
            Vector3 targetPos = player.transform.position + Vector3.up * 1.0f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                //// Add juice to fridge when packet reaches player
                //if (juiceData != null)
                //    JuiceFridgeManager.Instance.AddJuice(juiceData, 1); // Event will update UI automatically

                // Remove fruit from scene
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            gameObject.SetActive(false);
        }
    }

    // Detect clicks/taps on the fruit using raycast
    void DetectFruitTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject && isGrown)
                {
                    if (player == null)
                    {
                        Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
                        return;
                    }

                    var playerController = player.GetComponent<PlayerController>();

                    // Let the player decide if it can collect the fruit
                    if (playerController.TryCollectFruit())  moveToPlayer = true;
                }
            }
        }
    }

}
