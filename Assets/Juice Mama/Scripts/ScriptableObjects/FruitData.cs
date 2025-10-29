using UnityEngine;

[CreateAssetMenu(menuName = "Data/FruitSO")]
public class FruitData : ItemData
{
    [Header("Growth Settings")]
    public float growthTime = 10f;
    public int baseMaxFruits = 3;
    
    [Header("Juice Properties")]
    public float juiceYield = 1f;
    public Color juiceColor = Color.red;
    
    [Header("Visual Settings")]
    public Color fruitColor = Color.white;
    public ParticleSystem harvestEffect;
    
    [Header("Economic Settings")]
    public int baseValue = 10;
    public float upgradeCostMultiplier = 1.5f;
}