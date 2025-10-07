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
    public int maxFruitCapacity = 3;
    public bool canCarry = true;
    //*

    void Start()
    {
        if (!playerAnimator) playerAnimator = GetComponentInChildren<Animator>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fruit") && canCarry )
        {           
            fruitsCarried ++;
            if (fruitsCarried >= maxFruitCapacity)
            {
                canCarry = false;
            }
        }
    }
}
