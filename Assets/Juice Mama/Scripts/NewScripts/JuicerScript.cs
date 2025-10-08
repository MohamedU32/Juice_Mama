using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceMachine : MonoBehaviour
{
    public GameObject juicePacketPrefab; // Assign in Inspector
    private Transform spawnPointsParent;
    private List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        // Find the SpawnPoints GameObject directly under JuicingMachine
        spawnPointsParent = transform.Find("SpawnPoints");

        if (spawnPointsParent != null)
        {
            foreach (Transform child in spawnPointsParent)
            {
                spawnPoints.Add(child);
            }
        }
        else
        {
            Debug.LogWarning("SpawnPoints object not found under JuicingMachine.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(SpawnJuicePackets(playerController.fruitsCarried));
            }
        }
    }

    private IEnumerator SpawnJuicePackets(int fruitCount)
    {
        int packetsSpawned = 0;

        while (packetsSpawned < fruitCount)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(juicePacketPrefab, spawnPoint.position, Quaternion.identity);
                packetsSpawned++;

                if (packetsSpawned >= fruitCount)
                    yield break;

                yield return new WaitForSeconds(2f);
            }
        }
    }
}
