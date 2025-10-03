using UnityEngine;
using System;

public class Apple : MonoBehaviour
{
    public event Action<Apple> OnClicked;
    private bool isHarvested = false; // Prevent double harvesting

    public bool IsHarvested => isHarvested;

    private void OnMouseDown()
    {
        if (isHarvested) return; // Ignore multiple clicks
        isHarvested = true;

        OnClicked?.Invoke(this); // Notify TreeController
    }
}
