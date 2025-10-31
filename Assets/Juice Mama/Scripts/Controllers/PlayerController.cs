using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f;
    [SerializeField] private FloatingJoystick_Custom joystick;

    private StorageController playerStorage;

    private Transform farmAndStoreSeparator;
    private bool isPlayerInStore = false;
    private bool lastPlayerInStoreState = false;


    private float horizontalInput;
    private float verticalInput;

    public Animator playerAnimator;
    public int maxFruitCapacity = 4;
    public bool canCarryFruit => playerStorage == null ? false : (playerStorage.GetFruitCount() < maxFruitCapacity);

    public int maxJuiceCapacity = 4;

    public bool canCarryJuice => playerStorage == null ? false : (playerStorage.GetJuiceCount() < maxJuiceCapacity);

    void Awake()
    {
        playerStorage = gameObject.GetComponent<StorageController>();
        if (playerStorage == null)
        {
            Debug.LogError("StorageController component not found on Player.");
        }
        if (!playerAnimator) playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        AudioManager.Instance.PlaySound(AudioNames.BACKGROUND_MUSIC, 0.3f, true);
        farmAndStoreSeparator = GameObject.Find("FarmAndStoreSeparator").transform;
        CheckPlayerPosition();
        PlayAmbientSoundEffect();
    }

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

        CheckPlayerPosition();

        if (isPlayerInStore != lastPlayerInStoreState)
        {
            PlayAmbientSoundEffect();
            lastPlayerInStoreState = isPlayerInStore;
        }
    }

    // ✅ Handles fruit collection and triggers tutorial event
    public bool TryCollectFruit(FruitData fruitData)
    {
        if (canCarryFruit)
        {
            playerStorage.Add(fruitData, 1);
            UIManager.Instance.UpdateFruitCount();
            AudioManager.Instance.PlaySound(AudioNames.COLLECTED_FRUIT);

            // Trigger tutorial event for collecting fruit
            TutorialEventSystem.RaiseStepCompleted("CollectFruit");
            return true;
        }

        Debug.Log("Cannot carry more fruit!");
        AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION);
        return false;
    }

    // ✅ Handles juice collection and triggers tutorial event
    public bool TryCollectJuice(JuiceData juiceData)
    {
        if (canCarryJuice)
        {
            playerStorage.Add(juiceData, 1);
            UIManager.Instance.UpdateJuiceCount();
            AudioManager.Instance.PlaySound(AudioNames.COLLECTED_JUICE);

            // Trigger tutorial event for collecting juice
            TutorialEventSystem.RaiseStepCompleted("CollectJuice");
            return true;
        }

        Debug.Log("Cannot carry more juice!");
        AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION);
        return false;
    }

    public void CollectJuice(JuicerController juicerController)
    {
        if (playerStorage == null || juicerController == null) return;
        int count = maxJuiceCapacity - playerStorage.GetJuiceCount();
        if (count <= 0)
        {
            Debug.Log("Cannot carry more juice!");
            AudioManager.Instance.PlaySound(AudioNames.FAILED_COLLECTION);
            return;
        }
        juicerController.PickJuice(playerStorage, count);
        UIManager.Instance.UpdateFruitCount();
        UIManager.Instance.UpdateJuiceCount();
        AudioManager.Instance.PlaySound(AudioNames.COLLECTED_JUICE);
    }

    public void DepositJuiceInFridge(StorageController targetStorage)
    {
        if (playerStorage == null || targetStorage == null) return;
        List<JuiceData> juices = playerStorage.GetAllJuices();

        if (juices.Count == 0) return;

        for (int i = 0; i < juices.Count; i++)
        {
            int count = playerStorage.GetCount(juices[i]);
            if (count > 0)
            {
                int transferred = playerStorage.TransferItemsTo(targetStorage, juices[i], count);
                if (transferred > 0)
                {
                    UIManager.Instance.UpdateJuiceCount();
                    AudioManager.Instance.PlaySound(AudioNames.STORAGE_FILLED, 1.0f);
                }
            }
        }

        GameEvents.OnFridgeLoaded?.Invoke();
    }

    private void PlayAmbientSoundEffect ()
    {
        AudioManager.Instance.StopSound(AudioNames.AMBIENT_FARM);
        AudioManager.Instance.StopSound(AudioNames.AMBIENT_STORE);

        if (isPlayerInStore)
        {
            AudioManager.Instance.PlaySound(AudioNames.AMBIENT_STORE, 0.5f, true);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioNames.AMBIENT_FARM, 0.5f, true);
        }
    }

    private void CheckPlayerPosition ()
    {
        if (farmAndStoreSeparator != null)
        {
            isPlayerInStore = gameObject.transform.position.x >= farmAndStoreSeparator.position.x;
        }
    }

}
