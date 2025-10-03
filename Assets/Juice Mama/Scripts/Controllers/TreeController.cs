using UnityEngine;

public class TreeController : MonoBehaviour
{
    public AppleTreeData treeData;

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
        // spawn apples and subscribe click event immediately
        treeView.SpawnApples(treeModel.currentApples, treeData.applePrefab, transform, HandleAppleClicked);
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
