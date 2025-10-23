using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class WorkersManager : MonoBehaviour
{
    public static WorkersManager Instance { get; private set; }
    public GameObject workerPrefab;

}
