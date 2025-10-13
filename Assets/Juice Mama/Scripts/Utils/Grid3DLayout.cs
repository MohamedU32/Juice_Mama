using UnityEngine;

public class Grid3DLayout : MonoBehaviour
{
    public int columns = 4;
    public int rows = 4;
    public int layers = 1;
    public Vector3 spacing = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 origin = Vector3.zero;
    public Vector3 childScale = Vector3.one;

    void LateUpdate()
    {
        Arrange();
    }

    public void Arrange()
    {
        int i = 0;
        float totalWidth = (columns - 1) * spacing.x;
        float totalDepth = (rows - 1) * spacing.z;
        Vector3 centerOffset = new Vector3(totalWidth, 0f, totalDepth) * -0.5f;

        foreach (Transform child in transform)
        {
            int x = i % columns;
            int y = (i / (columns * rows));
            int z = (i / columns) % rows;
            float yPos = y * spacing.y;
            child.localPosition = origin + centerOffset + new Vector3(x * spacing.x, 0f, z * spacing.z) + new Vector3(0f, yPos, 0f);
            child.localScale = childScale;
            i++;
        }
    }
}
