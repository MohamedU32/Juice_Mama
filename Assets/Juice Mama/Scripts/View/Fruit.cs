using UnityEngine;
using System;

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitData fruitData;
    private bool isHarvested = false; // Prevent double harvesting

    public bool IsHarvested => isHarvested;

    private void OnMouseDown()
    {
        if (isHarvested) return; // Ignore multiple clicks
        isHarvested = true;

        GameEvents.OnFruitCollected?.Invoke(fruitData); // Notify TreeController
    }
}
