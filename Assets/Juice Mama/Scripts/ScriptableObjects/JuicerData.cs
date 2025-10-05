using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Juicer")]
public class JuicerData : ScriptableObject
{
    public string id;
    public string displayName;
    public float processTime;
    public JuiceData juiceData;
    public int outputCount = 1;
    public List<RecipeEntry> recipe = new List<RecipeEntry>();
}

[System.Serializable]
public class RecipeEntry
{
    public FruitData fruit;
    public int count = 1;
}
