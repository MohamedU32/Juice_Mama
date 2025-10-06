using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("References")]
    public FruitData fruitData;  // Assigned by TreeView when spawning
    public TreeView treeView;    // Assigned when spawned

    private bool isHarvested = false; 
    public bool IsHarvested => isHarvested;

    private void OnMouseDown()
    {
        if (isHarvested) return; // Prevent double harvesting
        isHarvested = true;

        // Remove fruit from tree
        treeView?.RemoveOneFruit(gameObject);

        // Add to inventory
        FruitCollectionManager.Instance.AddFruit(fruitData, 1);

        // Optional: Notify other systems
        GameEvents.OnFruitCollected?.Invoke(fruitData);
    }
}
