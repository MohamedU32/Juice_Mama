using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tree")]
public class TreeData : ScriptableObject
{
    public string id;
    public string displayName;
    public int level;
    public FruitData fruitData;

    [Header("Growth Settings")]
    public float growthTime;
    public int count = 0;
    public int maxCount = 4;

    [Header("Upgrade Settings")]
    public int maxUpgradeLevel = 5;
    public float growthTimeMultiplierPerLevel = 0.9f;
    public int extraFruitPerLevel = 1;
}
