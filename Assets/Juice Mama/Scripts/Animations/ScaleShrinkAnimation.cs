using UnityEngine;

public class ScaleShrinkAnimation : MonoBehaviour
{
    [SerializeField] private float min = 0.5f;
    [SerializeField] private float max = 1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnStart = true;
    Vector3 orig;
    bool playing;
    float t;
    void Awake() { orig = transform.localScale; if (playOnStart) Play(); }
    void OnEnable() { if (playOnStart) Play(); }
    void OnDisable() { Pause(); }
    public void Play() { playing = true; t = 0f; }
    public void Pause() { playing = false; transform.localScale = orig; }
    void Update()
    {
        if (!playing) return;
        t += Time.deltaTime * speed;
        float s = Mathf.Lerp(max, min, 0.5f * (1 + Mathf.Sin(t)));
        transform.localScale = orig * s;
        if (!loop && t >= Mathf.PI) { Pause(); }
    }
}
