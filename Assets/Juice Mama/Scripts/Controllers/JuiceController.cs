using UnityEngine;
using System.Collections;
using System.Linq;

public class JuicerController : MonoBehaviour
{
    public JuicerData juicerData;
    public Transform juiceSpawnPoint;
    public GameObject processingEffect;

    private bool isProcessing = false;
    private bool playerInRange = false;
    private int fruitsDeposited = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryDepositFruits();
        }
    }

    private void TryDepositFruits()
    {
        if (juicerData == null || juicerData.recipe == null)
        {
            Debug.LogWarning("Juicer has no data or recipe!");
            return;
        }

        bool depositedSomething = false;

        foreach (var recipeItem in juicerData.recipe)
        {
            int available = FruitCollectionManager.Instance.GetFruitCount(recipeItem.fruit);
            int required = recipeItem.count;

            if (available >= required)
            {
                FruitCollectionManager.Instance.AddFruit(recipeItem.fruit, -required);
                fruitsDeposited += required;
                depositedSomething = true;
                Debug.Log($"Deposited {required} {recipeItem.fruit.displayName}!");
            }
            else
            {
                Debug.Log($"Not enough {recipeItem.fruit.displayName}. Need {required}, have {available}");
            }
        }

        if (depositedSomething && !isProcessing && fruitsDeposited >= RequiredFruitCount())
        {
            StartCoroutine(ProcessJuice());
        }
    }

    private int RequiredFruitCount()
    {
        return juicerData.recipe.Sum(r => r.count);
    }

    private IEnumerator ProcessJuice()
    {
        isProcessing = true;
        Debug.Log($"Processing {juicerData.displayName}...");

        if (processingEffect) processingEffect.SetActive(true);
        yield return new WaitForSeconds(juicerData.processTime);
        if (processingEffect) processingEffect.SetActive(false);

        SpawnJuice();
        fruitsDeposited = 0;
        isProcessing = false;
    }

    private void SpawnJuice()
    {
        if (juicerData.juiceData == null || juicerData.juiceData.juicePrefab == null)
        {
            Debug.LogWarning("Missing juice data or prefab!");
            return;
        }

        GameObject juice = Instantiate(
            juicerData.juiceData.juicePrefab,
            juiceSpawnPoint.position,
            Quaternion.identity
        );

        Debug.Log($"Created {juicerData.juiceData.displayName}!");
        GameEvents.OnJuiceProcessed?.Invoke(juicerData.juiceData);
    }
}
