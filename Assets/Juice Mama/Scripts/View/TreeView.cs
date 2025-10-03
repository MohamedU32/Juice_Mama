using UnityEngine;
using System.Collections.Generic;
using System;

public class TreeView : MonoBehaviour
{
    private List<GameObject> activeApples = new List<GameObject>();

    public void SpawnApples(int count, GameObject applePrefab, Transform treeTransform, Action<Apple> onAppleClicked)
    {
        ClearAllApples(); // Remove previous apples

        float radius = 2f; // How far apples can spawn from tree center
        float minHeight = 2f;
        float maxHeight = 5f;

        for (int i = 0; i < count; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = UnityEngine.Random.Range(0.5f * radius, radius);
            float height = UnityEngine.Random.Range(minHeight, maxHeight);

            Vector3 spawnPos = treeTransform.position + new Vector3(
                Mathf.Cos(angle) * distance,
                height,
                Mathf.Sin(angle) * distance
            );

            GameObject appleObj = Instantiate(applePrefab, spawnPos, Quaternion.identity, treeTransform);
            activeApples.Add(appleObj);

            Apple apple = appleObj.GetComponent<Apple>();
            if (apple != null)
            {
                apple.OnClicked += onAppleClicked; // Subscribe safely
            }
        }
    }

    public void RemoveOneApple()
    {
        if (activeApples.Count == 0) return;

        GameObject apple = activeApples[0];
        activeApples.RemoveAt(0);
        Destroy(apple); // Destroying GameObject automatically removes all event subscribers
    }

    public void ClearAllApples()
    {
        foreach (var appleObj in activeApples)
        {
            Destroy(appleObj); // Safe: events are removed with GameObject
        }
        activeApples.Clear();
    }
}
