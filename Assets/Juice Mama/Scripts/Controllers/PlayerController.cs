using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f;
    [SerializeField] private FloatingJoystick_Custom joystick;

    private float horizontalInput;
    private float verticalInput;

    public Animator playerAnimator;

    //#
    public AudioSource playerAudioSource;

    public int fruitsCarried = 0;
    public int maxFruitCapacity = 4;
    public AudioClip collectedFruitSoundEffect;
    public bool canCarryFruit => fruitsCarried < maxFruitCapacity;
    
    public int juicesCarried = 0;
    public int maxJuiceCapacity = 4;
    public AudioClip collectedJuiceSoundEffect;

    public AudioClip FailedCollectionSoundEffect;

    public bool canCarryJuice => juicesCarried < maxJuiceCapacity;
    //*

    void Start()
    {
        if (!playerAnimator) playerAnimator = GetComponentInChildren<Animator>();
        UIManager.Instance.UpdateFruitCount(fruitsCarried, maxFruitCapacity);
        UIManager.Instance.UpdateJuiceCount(juicesCarried, maxJuiceCapacity);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = joystick.Horizontal;
        verticalInput = joystick.Vertical;

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        if (movement.magnitude > 0.1f)
        {
            transform.forward = movement;
            playerAnimator.SetBool("isWalking", true);
        }
        else playerAnimator.SetBool("isWalking", false);
    }

    public bool TryCollectFruit()
    {
        if (canCarryFruit)
        {
            fruitsCarried++;
            UIManager.Instance.UpdateFruitCount(fruitsCarried, maxFruitCapacity);
            playerAudioSource.PlayOneShot(collectedFruitSoundEffect, 1f);
            return true;
        }

        Debug.Log("Cannot carry more fruit!");
        playerAudioSource.PlayOneShot(FailedCollectionSoundEffect, 1f);
        return false;
    }

    public bool TryCollectJuice()
    {
        if (canCarryJuice)
        {
            juicesCarried++;
            UIManager.Instance.UpdateJuiceCount(juicesCarried, maxJuiceCapacity);
            playerAudioSource.PlayOneShot(collectedJuiceSoundEffect, 1f);
            return true;
        }

        Debug.Log("Cannot carry more juice!");
        playerAudioSource.PlayOneShot(FailedCollectionSoundEffect, 1f);
        return false;
    }
}
