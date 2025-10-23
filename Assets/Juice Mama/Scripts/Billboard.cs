using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;
        var f = cam.transform.forward;
        f.y = 0f;
        if (f.sqrMagnitude < 1e-6f) return;
        var y = Quaternion.LookRotation(f).eulerAngles.y;
        var e = transform.eulerAngles;
        e.y = y;
        transform.eulerAngles = e;
    }
}