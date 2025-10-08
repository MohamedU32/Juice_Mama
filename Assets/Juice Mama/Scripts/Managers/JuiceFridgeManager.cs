using UnityEngine;
using System.Collections.Generic;

public class JuiceFridgeManager : MonoBehaviour
{
    public static JuiceFridgeManager Instance { get; private set; }

    [Header("Fridge Storage")]
    public List<JuiceFridgeData> fridgeSlots = new List<JuiceFridgeData>();

    private Dictionary<JuiceData, int> juiceCounts = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize counts
        foreach (var slot in fridgeSlots)
        {
            if (slot.juiceData != null)
                juiceCounts[slot.juiceData] = slot.count;
        }
    }

    public void AddJuice(JuiceData juice, int amount = 1)
    {
        if (juice == null) return;

        if (!juiceCounts.ContainsKey(juice))
        {
            juiceCounts[juice] = 0;

            // Optional: create a fridge slot dynamically
            JuiceFridgeData newSlot = ScriptableObject.CreateInstance<JuiceFridgeData>();
            newSlot.juiceData = juice;
            newSlot.count = 0;
            newSlot.displayName = juice.displayName;
            fridgeSlots.Add(newSlot);
        }

        juiceCounts[juice] += amount;
        Debug.Log($"Added {amount} {juice.displayName} to fridge. Total: {juiceCounts[juice]}");

        // Update the data file count (optional)
        var slotData = fridgeSlots.Find(s => s.juiceData == juice);
        if (slotData != null)
            slotData.count = juiceCounts[juice];
    }

    public int GetJuiceCount(JuiceData juice)
    {
        return juiceCounts.ContainsKey(juice) ? juiceCounts[juice] : 0;
    }
}
