using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TreeController : MonoBehaviour
{
    public TreeData treeData;
    [Tooltip("Optional: Place spawn points as children of this GameObject and assign here.")]
    public Transform[] spawnPoints;
    private List<GameObject> spawnedFruits = new List<GameObject>();

    private TreeModel treeModel;
    private TreeView treeView;

    public void SetTreeView(TreeView view)
    {
        treeView = view;
    }

    void Awake()
    {
        if (treeView == null)
        {
            treeView = GetComponent<TreeView>();
            if (treeView == null) treeView = gameObject.AddComponent<TreeView>();
        }

        if (treeData == null)
        {
            Debug.LogError("TreeData not assigned!", this);
            enabled = false;
            return;
        }

        //  Auto-detect spawn points if none were assigned in Inspector
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = GetComponentsInChildren<Transform>()
                .Where(t => t != this.transform) // ignore the tree root
                .ToArray();

            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points found! Please add child empty GameObjects as spawn points.", this);
            }
        }

        treeModel = new TreeModel(treeData);

        treeModel.OnGrowthStarted += OnGrowthStarted;
        treeModel.OnGrowthCompleted += OnGrowthCompleted;
        GameEvents.OnFruitCollected += OnFruitCollected;

        treeModel.StartGrowth();
    }

    void Update()
    {
        if (treeModel != null && treeModel.IsGrowthTimeElapsed())
            treeModel.CompleteGrowth();
    }

    private void OnGrowthStarted() => Debug.Log("Growth started!");

    private void OnGrowthCompleted()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        treeView.SpawnFruit(
            treeModel.currentFruits,
            treeData.fruitData.fruitPrefab,
            spawnPoints
        );
    }

    private void OnFruitCollected(FruitData fruitData)
    {
        if (fruitData != null)
        {
            if (fruitData.id == treeData.fruitData.id)
            {
                treeModel.HarvestFruit();
                treeView.RemoveOneFruit();
            }
        }
    }

    void OnDestroy()
    {
        if (treeModel != null)
        {
            treeModel.OnGrowthStarted -= OnGrowthStarted;
            treeModel.OnGrowthCompleted -= OnGrowthCompleted;
        }
    }
}
