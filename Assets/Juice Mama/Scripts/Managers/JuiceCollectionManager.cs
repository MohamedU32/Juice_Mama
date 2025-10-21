using System.Collections.Generic;
using UnityEngine;

public class JuiceCollectionManager : MonoBehaviour
{
    public static JuiceCollectionManager Instance { get; private set; }
    private Dictionary<JuiceData, int> juiceInventory = new();

    public event System.Action OnJuiceChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddJuice(JuiceData juice, int amount)
    {
        if (juice == null || amount <= 0) return;

        if (!juiceInventory.ContainsKey(juice))
            juiceInventory[juice] = 0;

        juiceInventory[juice] += amount;
        OnJuiceChanged?.Invoke();
    }

    public bool RemoveJuice(JuiceData juice, int amount)
    {
        if (juice == null || !juiceInventory.ContainsKey(juice) || juiceInventory[juice] < amount)
            return false;

        juiceInventory[juice] -= amount;
        if (juiceInventory[juice] <= 0)
            juiceInventory.Remove(juice);

        OnJuiceChanged?.Invoke();
        return true;
    }

    public Dictionary<JuiceData, int> GetAllJuice()
    {
        return new Dictionary<JuiceData, int>(juiceInventory);
    }
}
