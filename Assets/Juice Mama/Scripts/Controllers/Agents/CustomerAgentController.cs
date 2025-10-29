using UnityEngine;
using UnityEngine.AI;
using MamaJuice.Utilities;

public class CustomerAgentController : AgentController
{
    [SerializeField] private Transform patienceBarRoot;
    [SerializeField] private SpriteRenderer patienceBarFill;
    [SerializeField] private float patienceSeconds = 30f;
    public Transform charactersTransform;
    private Timer patienceTimer;
    public StandQueueController standQueueController;
    private bool barInit;
    private Vector3 baseScale;
    private float basePosX;
    private float spriteWidth;

    public void SetStand(StandQueueController stand)
    {
        standQueueController = stand;
    }

    private void Awake()
    {
        if (charactersTransform != null)
        {
            int count = charactersTransform.childCount;
            if (count > 0)
            {
                int selectedIndex = Random.Range(0, count);
                for (int i = 0; i < count; i++)
                {
                    Transform child = charactersTransform.GetChild(i);
                    child.gameObject.SetActive(i == selectedIndex);
                    if (i == selectedIndex)
                    {
                        animator = child.GetComponent<Animator>();
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        if (patienceTimer != null)
        {
            patienceTimer.Stop();
            patienceTimer = null;
        }
        SetBar(1f);
        ShowBar(false);
    }

    void StartPatience()
    {
        if (patienceTimer != null) patienceTimer.Stop();
        patienceTimer = Timer.StartNew(patienceSeconds, LeaveDueToImpatience);
        ShowBar(true);
        InitBar();
        SetBar(1f);
    }

    new void Update()
    {
        base.Update();
        if (isWaiting && (standQueueController != null) && standQueueController.IsInFront(this.agent))
        {
            if (patienceTimer == null || !patienceTimer.IsRunning) StartPatience();
            var denom = patienceSeconds > 0f ? patienceSeconds : 1f;
            var value = patienceTimer != null ? patienceTimer.Remaining / denom : 0f;
            SetBar(value);
        }
        else
        {
            if (patienceTimer != null)
            {
                patienceTimer.Stop();
                patienceTimer = null;
            }
            SetBar(1f);
            ShowBar(false);
        }
    }

    void LeaveDueToImpatience()
    {
        if (standQueueController != null)
        {
            CustomersManager.Instance.OnCustomerServed(standQueueController.DequeueFront());
        }
    }

    void SetBar(float t)
    {
        if (patienceBarFill == null) return;
        if (!barInit) InitBar();
        t = Mathf.Clamp01(t);
        var newX = Mathf.Max(0.0001f, baseScale.x * t);
        var s = patienceBarFill.transform.localScale;
        s.x = newX;
        s.y = baseScale.y;
        s.z = baseScale.z;
        patienceBarFill.transform.localScale = s;
        var lp = patienceBarFill.transform.localPosition;
        lp.x = basePosX + (spriteWidth * 0.5f) * (s.x - baseScale.x);
        patienceBarFill.transform.localPosition = lp;
    }

    void ShowBar(bool show)
    {
        if (patienceBarRoot != null) patienceBarRoot.gameObject.SetActive(show);
    }

    void InitBar()
    {
        if (patienceBarFill == null || barInit) return;
        baseScale = patienceBarFill.transform.localScale;
        basePosX = patienceBarFill.transform.localPosition.x;
        spriteWidth = patienceBarFill.sprite != null ? patienceBarFill.sprite.bounds.size.x : 1f;
        barInit = true;
    }
}