using UnityEngine;

[CreateAssetMenu(menuName = "Data/EmployeeJobSO")]
public class EmployeeJobData : ScriptableObject
{
    public string id;
    public FruitData fruitData;
    public JuiceData juiceData;
    public string treeTag;
    public string juicerTag;
    public string fridgeTag;
    public float hirePrice;
    public GameObject employeePrefab;
    public int totalHired = 0;
}