using UnityEngine;

public class TranslateUpDownAnimation : MonoBehaviour
{
    [SerializeField] private float distance = 0.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnStart = true;
    Vector3 orig;
    bool playing;
    float t;
    void Awake() { orig = transform.localPosition; if (playOnStart) Play(); }
    void OnEnable() { if (playOnStart) Play(); }
    void OnDisable() { Pause(); }
    public void Play() { playing = true; t = 0f; }
    public void Pause() { playing = false; transform.localPosition = orig; }
    void Update()
    {
        if (!playing) return;
        t += Time.deltaTime * speed;
        float y = Mathf.Sin(t) * distance;
        transform.localPosition = orig + Vector3.up * y;
        if (!loop && t >= Mathf.PI) { Pause(); }
    }
}
