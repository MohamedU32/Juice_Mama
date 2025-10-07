using Unity.VisualScripting;
using UnityEngine;

public class FruitGrow : MonoBehaviour
{
    public bool isGrown = false;
    [SerializeField] private Vector3 targetScale = new Vector3(4, 4, 4);
    [SerializeField] private float scaleValue = 0.005f;

    [SerializeField] private float speed = 10.0f;
    private GameObject player;
    private bool moveToPlayer = false;
    public AudioClip collectedSoundEffect;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrown)
        {
            transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
        }

        if (transform.localScale.x >= targetScale.x)
        {
            isGrown = true;
            transform.Rotate(Vector3.up * Time.deltaTime * speed );
        }

        if (moveToPlayer && player != null)
        {
            Vector3 targetTransform = player.transform.position + new Vector3(0, 1.0f, 0);
            transform.position = Vector3.MoveTowards(transform.position, targetTransform, speed * Time.deltaTime);
            //if (transform.position == targetTransform) { gameObject.SetActive(false); }
        }
    }

    private void OnMouseDown()
    {
        if (isGrown)
        {
            moveToPlayer = true;
            player.GetComponent<PlayerController>().playerAudioSource.PlayOneShot(collectedSoundEffect, 1.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
