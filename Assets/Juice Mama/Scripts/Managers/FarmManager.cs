using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public GameObject treePrefab;      // assign Tree prefab
    public GameObject applePrefab;     // assign Apple prefab
    public TreeData treeData;     // assign TreeData ScriptableObject

    private Vector3[] plotPositions = new Vector3[]
    {
        new Vector3(-13.5f, 0.25f, -5f),
        new Vector3(-6.5f, 0.25f, -5f),
        new Vector3(0f, 0.25f, -5f),
        new Vector3(-20f, 0.25f, 5f),
        new Vector3(-13.5f, 0.25f, 5f),
        new Vector3(-6.5f, 0.25f, 5f),
        new Vector3(0f, 0.25f, 5f)
    };

    void Start()
    {
        foreach (var plotPos in plotPositions)
        {
            SpawnTree(plotPos);
            SpawnTree(plotPos + new Vector3(2f, 0f, 0f)); // second tree slightly offset
        }
    }

    private void SpawnTree(Vector3 position)
    {
        GameObject treeObj = Instantiate(treePrefab, position, Quaternion.identity);
        TreeController controller = treeObj.GetComponent<TreeController>();
        if (controller != null)
        {
            controller.treeData = treeData;
            // Use a lambda to pass click handling directly
            TreeView view = treeObj.GetComponent<TreeView>();
            if (view != null)
            {
                controller.SetTreeView(view);
            }
        }
    }
}
