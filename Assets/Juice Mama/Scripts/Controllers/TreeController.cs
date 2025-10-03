using UnityEngine;
using System.Linq;

public class TreeController : MonoBehaviour
{
    public AppleTreeData treeData;
    [Tooltip("Optional: Place spawn points as children of this GameObject and assign here.")]
    public Transform[] appleSpawnPoints;

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

        // ✅ Auto-detect spawn points if none were assigned in Inspector
        if (appleSpawnPoints == null || appleSpawnPoints.Length == 0)
        {
            appleSpawnPoints = GetComponentsInChildren<Transform>()
                .Where(t => t != this.transform) // ignore the tree root
                .ToArray();

            if (appleSpawnPoints.Length == 0)
            {
                Debug.LogError("No apple spawn points found! Please add child empty GameObjects as spawn points.", this);
            }
        }

        treeModel = new TreeModel(treeData);

        treeModel.OnGrowthStarted += OnGrowthStarted;
        treeModel.OnGrowthCompleted += OnGrowthCompleted;
        treeModel.OnAppleHarvested += remaining => treeView.RemoveOneApple();

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
        if (appleSpawnPoints == null || appleSpawnPoints.Length == 0) return;

        treeView.SpawnApples(
            treeModel.currentApples,
            treeData.applePrefab,
            appleSpawnPoints,
            HandleAppleClicked
        );
    }

    private void HandleAppleClicked(Apple apple)
    {
        if (apple != null)
        {
            treeModel.HarvestApple();
            Destroy(apple.gameObject);
        }
    }

    void OnDestroy()
    {
        if (treeModel != null)
        {
            treeModel.OnGrowthStarted -= OnGrowthStarted;
            treeModel.OnGrowthCompleted -= OnGrowthCompleted;
            treeModel.OnAppleHarvested -= remaining => treeView.RemoveOneApple();
        }
    }
}
