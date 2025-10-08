using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    [SerializeField] float heightOffset = 1.5f;
    [SerializeField] float bobAmp = 0.25f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] UnityEvent onClick;
    [SerializeField] private GameObject hoverPrefab;
    private GameObject hover;
    Vector3 baseOffset;
    bool active;


    //#
    PlayerController playerController;
    [SerializeField] AudioClip soundEffect;
    //*
    
    void Awake()
    {
        var col = GetComponent<Collider>();
        if (!col)
        {
            var box = gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;
        }
        else col.isTrigger = true;
        if (!hoverPrefab)
        {
            hover = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        else
        {
            hover = Instantiate(hoverPrefab);
        }
        hover.transform.SetParent(transform);
        baseOffset = Vector3.up * heightOffset;
        hover.transform.position = transform.position + baseOffset;
        hover.name = name + "_Hover";
        var p = hover.AddComponent<HoverClickProxy>();
        p.interactible = this;
        hover.SetActive(false);
    }

    //#
    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    //*


    void LateUpdate()
    {
        if (!hover) return;
        if (!active)
        {
            if (hover.activeSelf) hover.SetActive(false);
            return;
        }
        if (!hover.activeSelf) hover.SetActive(true);
        float y = Mathf.Sin(Time.time * bobSpeed) * bobAmp;
        hover.transform.position = transform.position + baseOffset + Vector3.up * y;
        hover.transform.Rotate(0f, 45f * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) active = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) active = false;
    }

    public void Click()
    {
        if (active) onClick?.Invoke();
        //#
        playerController.playerAudioSource.PlayOneShot(soundEffect,1.0f);
        //*
    }
}

public class HoverClickProxy : MonoBehaviour
{
    public Interactible interactible;
    void Awake()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }
    void OnMouseDown()
    {
        interactible?.Click();
    }
}