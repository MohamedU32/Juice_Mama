using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField] private FruitData fruitData;
    
    [Header("Fruit Growing Settings")]
    public TreeController parentTree;
    public bool isGrown = false;
    public bool moveToPlayer = false;
    public bool isFruitTouched = false;
    [SerializeField] private Vector3 startScale = new Vector3(0.25f, 0.25f, 0.25f);
    [SerializeField] private Vector3 targetScale = new Vector3(4, 4, 4);

    private void Start()
    {
        transform.localScale = startScale;
    }

    private void Update()
    {
        // Grow the fruit
        if (!isGrown)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, fruitData.growSpeed * Time.deltaTime);
            if (transform.localScale.x >= targetScale.x) 
                isGrown = true;
        }

        // Move to player if collected
        if (moveToPlayer && PlayerController.Instance != null)
        {
            Vector3 targetPos = PlayerController.Instance.transform.position + Vector3.up * 1.0f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, fruitData.moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  Destroy(gameObject);
    }
}