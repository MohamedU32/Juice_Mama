using UnityEngine;
using System;
using System.Collections.Generic;

public class TreeView : MonoBehaviour
{
    private List<GameObject> activeApples = new List<GameObject>();

    public void SpawnApples(int count, GameObject applePrefab, Transform[] spawnPoints, Action<Apple> onAppleClicked)
    {
        ClearAllApples(); // Remove old apples

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned for apples!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            // Pick a random spawn point
            Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            GameObject appleObj = Instantiate(applePrefab, point.position, point.rotation, point);
            activeApples.Add(appleObj);

            Apple apple = appleObj.GetComponent<Apple>();
            if (apple != null)
            {
                apple.OnClicked += onAppleClicked;
            }
        }
    }

    public void RemoveOneApple()
    {
        if (activeApples.Count == 0) return;

        GameObject apple = activeApples[0];
        activeApples.RemoveAt(0);
        Destroy(apple);
    }

    public void ClearAllApples()
    {
        foreach (var apple in activeApples)
        {
            if (apple != null) Destroy(apple);
        }
        activeApples.Clear();
    }
}
