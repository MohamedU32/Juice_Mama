using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Inventory")]
public class InventoryData : ScriptableObject
{
    public List<FruitEntry> fruits = new List<FruitEntry>();
    public List<JuiceEntry> juices = new List<JuiceEntry>();
}

[System.Serializable]
public class FruitEntry
{
    public FruitData fruit;
    public int count;
}

[System.Serializable]
public class JuiceEntry
{
    public JuiceData juice;
    public int count;
}
