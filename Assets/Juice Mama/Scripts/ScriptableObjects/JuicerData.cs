using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/JuicerSO")]
public class JuicerData : ScriptableObject
{
    public string id;
    public string displayName;
    public float processTime;
    public int maxPerFruit = 5;
    public JuiceData juiceData;
    public int outputCount = 1;
    public List<RecipeEntry> recipe = new List<RecipeEntry>();
}

[System.Serializable]
public class RecipeEntry
{
    public FruitData fruitData;
    public int count = 1;
}
