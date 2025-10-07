using UnityEngine;
using System.Collections;
using System.Linq;

public class JuicerController : MonoBehaviour
{
    [Header("Juicer Settings")]
    public JuicerData juicerData;
    public Transform juiceSpawnPoint;
    public GameObject processingEffect;

    [Header("Player Interaction")]
    public KeyCode interactionKey = KeyCode.E;

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
        if (playerInRange && Input.GetKeyDown(interactionKey))
            DepositFruits();
    }

    private void DepositFruits()
    {
        if (juicerData?.recipe == null)
        {
            Debug.LogWarning("Juicer has no data or recipe assigned!");
            return;
        }

        bool anyDeposited = false;

        foreach (var recipeItem in juicerData.recipe)
        {
            int requiredAmount = recipeItem.count;
            int availableAmount = FruitCollectionManager.Instance.GetFruitCount(recipeItem.fruit);

            if (availableAmount >= requiredAmount)
            {
                if (FruitCollectionManager.Instance.RemoveFruit(recipeItem.fruit, requiredAmount))
                {
                    fruitsDeposited += requiredAmount;
                    anyDeposited = true;
                    Debug.Log($"Deposited {requiredAmount} {recipeItem.fruit.displayName}!");
                }
            }
            else
            {
                Debug.Log($"Not enough {recipeItem.fruit.displayName}. Need {requiredAmount}, have {availableAmount}.");
            }
        }

        if (anyDeposited && !isProcessing && fruitsDeposited >= RequiredFruitCount())
            StartCoroutine(ProcessJuice());
    }

    private int RequiredFruitCount()
    {
        return juicerData.recipe.Sum(r => r.count);
    }

    private IEnumerator ProcessJuice()
    {
        isProcessing = true;
        Debug.Log($"Processing {juicerData.displayName}...");

        if (processingEffect)
            processingEffect.SetActive(true);

        yield return new WaitForSeconds(juicerData.processTime);

        if (processingEffect)
            processingEffect.SetActive(false);

        SpawnJuice();
        fruitsDeposited = 0;
        isProcessing = false;
    }

    private void SpawnJuice()
    {
        if (juicerData.juiceData?.juicePrefab == null)
        {
            Debug.LogWarning("Juice prefab or data missing!");
            return;
        }

        GameObject juiceInstance = Instantiate(
            juicerData.juiceData.juicePrefab,
            juiceSpawnPoint.position,
            Quaternion.identity
        );

        Debug.Log($"Created {juicerData.juiceData.displayName}!");
        GameEvents.OnJuiceProcessed?.Invoke(juicerData.juiceData);
    }
}
