using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceMachine : MonoBehaviour
{
    public GameObject juicePacketPrefab; // Assign in Inspector
    private Transform spawnPointsParent;
    private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] AudioClip soundEffect;

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

    public void ProcessJuice()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null && playerController.fruitsCarried > 0)
        {
            StartCoroutine(SpawnJuicePackets(playerController.fruitsCarried));
            if (soundEffect != null)
            {
                playerController.playerAudioSource.PlayOneShot(soundEffect, 1.0f);
            }
        }
    }
}
