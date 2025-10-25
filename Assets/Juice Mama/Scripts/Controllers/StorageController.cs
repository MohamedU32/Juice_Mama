using UnityEngine;
using System.Collections.Generic;

public class StorageController : MonoBehaviour
{
    public List<ItemEntry> items = new List<ItemEntry>();

    public List<ItemEntry> GetItems => items;


    public int GetFruitCount()
    {
        int total = 0;
        foreach (var entry in items)
        {
            if (entry.item is FruitData)
            {
                total += entry.count;
            }
        }
        return total;
    }

    public List<FruitData> GetAllFruits()
    {
        List<FruitData> fruits = new List<FruitData>();
        foreach (var entry in items)
        {
            if (entry.item is FruitData fruitData)
            {
                fruits.Add(fruitData);
            }
        }
        return fruits;
    }

    public List<JuiceData> GetAllJuices()
    {
        List<JuiceData> juices = new List<JuiceData>();
        foreach (var entry in items)
        {
            if (entry.item is JuiceData juiceData)
            {
                juices.Add(juiceData);
            }
        }
        return juices;
    }

    public int GetJuiceCount()
    {
        int total = 0;
        foreach (var entry in items)
        {
            if (entry.item is JuiceData)
            {
                total += entry.count;
            }
        }
        return total;
    }

    public int GetTotalCount()
    {
        int total = 0;
        foreach (var entry in items)
        {
            total += entry.count;
        }
        return total;
    }

    public int GetCount(ItemData item)
    {
        var entry = items.Find(e => e.item == item);
        return entry != null ? entry.count : 0;
    }

    public bool Add(ItemData item, int count)
    {
        if (item == null || count <= 0) return false;
        var entry = items.Find(e => e.item == item);
        if (entry != null)
        {
            entry.count += count;
        }
        else
        {
            items.Add(new ItemEntry { item = item, count = count });
        }
        return true;
    }

    public bool Remove(ItemData item, int count)
    {
        if (item == null || count <= 0) return false;
        var entry = items.Find(e => e.item == item);
        if (entry != null && entry.count >= count)
        {
            entry.count -= count;
            if (entry.count == 0)
            {
                items.Remove(entry);
            }
            return true;
        }
        return false;
    }

    public int TransferItemsTo(StorageController targetStorage, ItemData item, int count)
    {
        if (targetStorage == null || item == null || count <= 0) return 0;
        var entry = items.Find(e => e.item == item);
        if (entry != null && entry.count > 0)
        {
            int transferableCount = Mathf.Min(entry.count, count);
            if (targetStorage.Add(item, transferableCount))
            {
                entry.count -= transferableCount;
                if (entry.count == 0)
                {
                    items.Remove(entry);
                }
                return transferableCount;
            }
        }
        return 0;
    }
}