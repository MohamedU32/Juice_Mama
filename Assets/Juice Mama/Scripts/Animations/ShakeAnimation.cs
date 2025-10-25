using UnityEngine;

public class ShakeAnimation : MonoBehaviour
{
    [SerializeField] private float strength = 0.1f;
    [SerializeField] private float speed = 20f;
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
        transform.localPosition = orig + (Random.insideUnitSphere * strength);
        if (!loop && t >= 1f) { Pause(); }
    }
}
