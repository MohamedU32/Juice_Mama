using UnityEngine;

[CreateAssetMenu(
    fileName = "NewAppleTreeData",              // Default file name when created
    menuName = "FarmGame/Trees/AppleTree",      // Path in the Create menu
    order = 1)]                                 // Optional: ordering in menu
public class AppleTreeData : ScriptableObject
{
    [Header("Fruit Settings")]
    public GameObject applePrefab;
    public int baseMaxApples = 3;
    public Color appleColor = Color.red;

    [Header("Growth Settings")]
    public float baseGrowthTime = 5f;

    [Header("Upgrade Settings")]
    public int maxUpgradeLevel = 5;
    public float growthTimeMultiplierPerLevel = 0.9f;
    public int extraApplesPerLevel = 1;
}
