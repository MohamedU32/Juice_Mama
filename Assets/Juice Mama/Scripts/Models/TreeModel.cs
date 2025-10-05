using UnityEngine;
using System;

public class TreeModel
{
    public TreeData treeData;
    public int upgradeLevel = 0;
    public int currentFruits = 0;
    public bool isGrowing = false;
    public float growthStartTime = 0f;

    public event Action OnGrowthStarted;
    public event Action OnGrowthCompleted;

    public TreeModel(TreeData data)
    {
        if (data == null)
        {
            Debug.LogError("TreeModel was created without a valid TreeData!");
            return;
        }
        treeData = data;
    }

    // MaxCount calculation
    public int MaxCount
    {
        get
        {
            if (treeData == null) return 0;
            return treeData.maxCount + (upgradeLevel * treeData.extraFruitPerLevel);
        }
    }

    // GrowthTime calculation
    public float GrowthTime
    {
        get
        {
            if (treeData == null) return 0f;
            return treeData.growthTime * Mathf.Pow(treeData.growthTimeMultiplierPerLevel, upgradeLevel);
        }
    }

    public void StartGrowth()
    {
        if (treeData == null)
        {
            Debug.LogWarning("Cannot start growth: treeData is null");
            return;
        }

        if (!isGrowing && currentFruits == 0)
        {
            isGrowing = true;
            growthStartTime = Time.time;
            OnGrowthStarted?.Invoke();
            Debug.Log($"Growth started! Will complete in {GrowthTime} seconds");
        }
    }

    public bool IsGrowthTimeElapsed()
    {
        if (treeData == null) return false;
        return isGrowing && (Time.time - growthStartTime >= GrowthTime);
    }

    public void CompleteGrowth()
    {
        if (treeData == null)
        {
            Debug.LogWarning("Cannot complete growth: treeData is null");
            return;
        }

        if (!isGrowing)
        {
            Debug.LogWarning("CompleteGrowth called but tree is not growing");
            return;
        }

        isGrowing = false;
        currentFruits = MaxCount;
        OnGrowthCompleted?.Invoke();
        Debug.Log($"Growth completed! {currentFruits} fruits spawned");
    }

    public void HarvestFruit()
    {
        if (treeData == null)
        {
            Debug.LogWarning("Cannot harvest fruit: treeData is null");
            return;
        }

        if (currentFruits > 0)
        {
            currentFruits--;
            Debug.Log($"Fruit harvested! {currentFruits} remaining");

            // Auto-start growth when all fruits are harvested
            if (currentFruits == 0)
            {
                StartGrowth();
            }
        }
        else
        {
            Debug.LogWarning("No fruits available to harvest");
        }
    }

    public void UpgradeTree()
    {
        if (treeData == null)
        {
            Debug.LogWarning("Cannot upgrade tree: treeData is null");
            return;
        }

        if (upgradeLevel < treeData.maxUpgradeLevel)
        {
            upgradeLevel++;
            Debug.Log($"Tree upgraded to level {upgradeLevel}! Max fruits: {MaxCount}, Growth time: {GrowthTime}s");
        }
        else
        {
            Debug.LogWarning($"Tree is already at max level ({treeData.maxUpgradeLevel})");
        }
    }

    // Helper method to get remaining growth time
    public float GetRemainingGrowthTime()
    {
        if (!isGrowing || treeData == null) return 0f;
        float elapsed = Time.time - growthStartTime;
        return Mathf.Max(0f, GrowthTime - elapsed);
    }

    // Helper method to get growth progress (0-1)
    public float GetGrowthProgress()
    {
        if (!isGrowing || treeData == null) return 0f;
        float elapsed = Time.time - growthStartTime;
        return Mathf.Clamp01(elapsed / GrowthTime);
    }
}