using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class JuicerController : MonoBehaviour
{
    [Header("Juicer Settings")]
    public JuicerData juicerData;
    public GameObject juicePacketPrefab;

    [Tooltip("Optional: Place spawn points as children of this GameObject and assign here.")]
    public Transform[] spawnPoints;

    public GameObject processingEffect;

    private bool isProcessing = false;
    private bool playerInRange = false;
    private int fruitsDeposited = 0;

    private void Awake()
    {
        // Auto-detect spawn points if none were assigned
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = GetComponentsInChildren<Transform>()
                .Where(t => t != this.transform)
                .ToArray();

            if (spawnPoints.Length == 0)
                Debug.LogError("No spawn points found! Please add child empty GameObjects as spawn points.");
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            DepositFruits();
    }

    private void DepositFruits()
    {
        if (juicerData?.recipe == null) return;

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
        }

        if (anyDeposited && !isProcessing && fruitsDeposited >= RequiredFruitCount())
            StartCoroutine(ProcessJuice());
    }

    private int RequiredFruitCount() => juicerData.recipe.Sum(r => r.count);

    private IEnumerator ProcessJuice()
    {
        isProcessing = true;
        if (processingEffect) processingEffect.SetActive(true);

        yield return new WaitForSeconds(juicerData.processTime);

        if (processingEffect) processingEffect.SetActive(false);

        SpawnJuicePackets(fruitsDeposited);
        fruitsDeposited = 0;
        isProcessing = false;
    }

    private void SpawnJuicePackets(int packetCount)
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || juicePacketPrefab == null) return;

        StartCoroutine(SpawnPacketsCoroutine(packetCount));
    }

    private IEnumerator SpawnPacketsCoroutine(int packetCount)
    {
        int spawned = 0;

        while (spawned < packetCount)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                GameObject packet = Instantiate(juicePacketPrefab, spawnPoint.position, Quaternion.identity);

                // Assign JuiceData so the packet can be collected
                var packetScript = packet.GetComponent<JuicePacketScript>();
                if (packetScript != null)
                    packetScript.juiceData = juicerData.juiceData;

                spawned++;

                if (spawned >= packetCount)
                    yield break;

                yield return new WaitForSeconds(0.2f); // optional stagger
            }
        }
    }

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
}
