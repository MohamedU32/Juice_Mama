using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<FruitData> OnFruitCollected;
    public static Action<JuiceData> OnJuiceProcessed;
    public static Action<JuiceData, int> OnJuiceSold;
    public static Action OnFridgeLoaded;
    public static Action<GameObject> OnTreeFruitGrown;
    public static Action<string> OnUnlockableAvailable;
    public static Action<string> OnItemUnlocked;
    public static Action<string> StorageUpdated;
}