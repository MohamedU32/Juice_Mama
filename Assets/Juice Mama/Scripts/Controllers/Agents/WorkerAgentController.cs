using System.Collections.Generic;
using UnityEngine;

public class WorkerAgentController : AgentController
{
    public EmployeeJobData job;

    StorageController storage;
    JuicerController juicer;
    public int maxFruitCapacity = 4;
    public int maxJuiceCapacity = 4;
    Transform tree;
    List<Transform> trees = new List<Transform>();
    float nextTreesRefreshAt;
    StorageController fridgeStorage;
    enum State { Idle, GoTree, Pick, GoJuicer, Deposit, WaitJuice, GoCollect, GoFridge, Store }
    State state;
    bool juiceReady;


    void OnEnable()
    {
        GameEvents.OnJuiceProcessed += OnJuiceProcessed;
        GameEvents.OnTreeFruitGrown += OnTreeFruitGrown;
    }

    void OnDisable()
    {
        GameEvents.OnJuiceProcessed -= OnJuiceProcessed;
        GameEvents.OnTreeFruitGrown -= OnTreeFruitGrown;
    }

    new void Start()
    {
        base.Start();
        storage = GetComponent<StorageController>();
        state = State.Idle;
        RefreshTrees(true);
        InvokeRepeating(nameof(StateSequence), 1f, 0.5f);
    }

    void StateSequence()
    {
        RefreshTrees(false);
        switch (state)
        {
            case State.Idle:
                if (SelectTreeWithGrownFruit()) GoTree();
                else if (juiceReady) GoCollect();
                break;
            case State.GoTree:
                if (Arrived()) state = State.Pick;
                break;
            case State.Pick:
                if (!TryPickFruit() || storage.GetFruitCount() >= maxFruitCapacity || !HasGrownFruit(tree))
                {
                    GoJuicer();
                }
                break;
            case State.GoJuicer:
                if (Arrived()) state = State.Deposit;
                break;
            case State.Deposit:
                if (juicer != null)
                {
                    juicer.FillStorage(storage);
                }
                state = State.WaitJuice;
                break;
            case State.WaitJuice:
                if (juiceReady) GoCollect();
                break;
            case State.GoCollect:
                if (Arrived()) state = State.GoCollect;
                {
                    if (TryCollectJuice())
                    {
                        GoFridge();
                    }
                    else
                    {
                        state = State.WaitJuice;
                    }
                }
                break;
            case State.GoFridge:
                if (Arrived()) state = State.Store;
                break;
            case State.Store:
                StoreAllJuice();
                state = State.Idle;
                break;
        }
    }
    void GoTree()
    {
        if (!tree) { state = State.Idle; return; }
        MoveTo(tree.position);
        state = State.GoTree;
    }

    void GoJuicer()
    {
        if (!FindJuicer()) { state = State.Idle; return; }
        MoveTo(juicer.transform.position);
        state = State.GoJuicer;
    }

    void GoCollect()
    {
        juiceReady = false;
        if (!FindJuicer()) { state = State.Idle; return; }
        MoveTo(juicer.transform.position);
        state = State.GoCollect;
    }

    void GoFridge()
    {
        if (!FindFridge()) { state = State.Idle; return; }
        MoveTo(fridgeStorage.transform.position);
        state = State.GoFridge;
    }

    void RefreshTrees(bool force)
    {
        if (job == null || string.IsNullOrEmpty(job.treeTag)) return;
        if (!force && Time.time < nextTreesRefreshAt && trees.Count > 0) return;
        trees.Clear();
        var objs = GameObject.FindGameObjectsWithTag(job.treeTag);
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].GetComponent<TreeController>() == null) continue;
            var any = objs[i].GetComponentsInChildren<FruitController>(true);
            bool match = false;
            for (int k = 0; k < any.Length; k++)
            {
                var f = any[k];
                if (f.fruitData == job.fruitData) { match = true; break; }
            }
            if (match) trees.Add(objs[i].transform);
        }
        nextTreesRefreshAt = Time.time + 5f;
    }

    bool HasGrownFruit(Transform t)
    {
        if (t == null) return false;
        var fruits = t.GetComponentsInChildren<FruitController>(true);
        for (int i = 0; i < fruits.Length; i++)
        {
            var f = fruits[i];
            if (f.fruitData == job.fruitData && f.isGrown && f.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    bool SelectTreeWithGrownFruit()
    {
        if (trees.Count == 0) RefreshTrees(true);
        Transform best = null;
        float d = float.MaxValue;
        for (int i = 0; i < trees.Count; i++)
        {
            var t = trees[i];
            if (!t) continue;
            if (!HasGrownFruit(t)) continue;
            var dd = Vector3.SqrMagnitude(transform.position - t.position);
            if (dd < d) { d = dd; best = t; }
        }
        tree = best;
        return tree != null;
    }

    bool FindJuicer()
    {
        if (juicer != null) return true;
        if (job == null || string.IsNullOrEmpty(job.juicerTag)) return false;
        var objs = GameObject.FindGameObjectsWithTag(job.juicerTag);
        for (int i = 0; i < objs.Length; i++)
        {
            var s = objs[i].GetComponentInChildren<JuicerController>();
            if ((s == null) || (s.GetJuicerData().juiceData != job.juiceData)) continue;
            juicer = s;
        }
        return juicer != null;
    }

    bool FindFridge()
    {
        if (fridgeStorage != null) return true;
        if (job == null || string.IsNullOrEmpty(job.fridgeTag)) return false;
        var objs = GameObject.FindGameObjectsWithTag(job.fridgeTag);
        for (int i = 0; i < objs.Length; i++)
        {
            var controller = objs[i].GetComponent<StorageController>();
            if (controller == null) continue;
            fridgeStorage = controller;
        }
        return fridgeStorage != null;
    }

    bool TryPickFruit()
    {
        if (tree == null || job == null || job.fruitData == null || storage == null) return false;
        var fruits = tree.GetComponentsInChildren<FruitController>();
        for (int i = 0; i < fruits.Length; i++)
        {
            var fruit = fruits[i];
            if (fruit.fruitData == job.fruitData && fruit.isGrown && (storage.GetFruitCount() < maxFruitCapacity))
            {
                storage.Add(job.fruitData, 1);
                Destroy(fruit.gameObject);
                return true;
            }
        }
        return false;
    }

    bool TryCollectJuice()
    {
        if (storage == null || job == null || job.juiceData == null || juicer == null) return false;
        if (storage.GetCount(job.juiceData) < maxJuiceCapacity)
        {
            juicer.PickJuice(storage, Mathf.Max(maxJuiceCapacity - storage.GetCount(job.juiceData), 1));
            UIManager.Instance.UpdateJuiceCount();
            return true;
        }
        return false;
    }

    void StoreAllJuice()
    {
        if (storage == null || fridgeStorage == null) return;
        var juices = storage.GetAllJuices();
        for (int i = 0; i < juices.Count; i++)
        {
            var count = storage.GetCount(juices[i]);
            if (count > 0)
            {
                storage.TransferItemsTo(fridgeStorage, juices[i], count);
                GameEvents.OnFridgeLoaded?.Invoke();
            }
        }
    }

    void OnJuiceProcessed(JuiceData data)
    {
        if (job != null && data == job.juiceData) juiceReady = true;
    }

    void OnTreeFruitGrown(GameObject treeGo)
    {
        var treeController = treeGo.GetComponent<TreeController>();
        if (job != null && treeController?.GetTreeData()?.fruitData == job.fruitData)
        {
            if (state == State.Idle) { tree = treeGo.transform; GoTree(); }
        }
    }
}
