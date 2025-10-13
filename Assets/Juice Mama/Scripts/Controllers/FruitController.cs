using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField] private FruitData fruitData;
    [Header("Fruit Growing Settings")]
    public bool isGrown = false;
    [SerializeField] private Vector3 startScale = new Vector3(0.25f, 0.25f, 0.25f);
    [SerializeField] private Vector3 targetScale = new Vector3(4, 4, 4);
    private float scaleValue = 0.005f;

    [Header("Fruit Settings")]
    private GameObject player;
    private bool moveToPlayer = false;

    private void Start()
    {
        transform.localScale = startScale;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("Player not found! Make sure the player is tagged 'Player'.");
    }

    private void Update()
    {
        if (!isGrown)
        {
            transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
            if (transform.localScale.x >= targetScale.x) isGrown = true;
        }

        DetectFruitTouch();

        if (moveToPlayer && player != null)
        {
            Vector3 targetPos = player.transform.position + Vector3.up * 1.0f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fruitData.growthTime * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
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
                    if (playerController.TryCollectFruit(fruitData)) moveToPlayer = true;
                }
            }
        }
    }
}
