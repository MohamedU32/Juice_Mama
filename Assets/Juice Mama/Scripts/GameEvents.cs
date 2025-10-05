using System;

public static class GameEvents
{
    public static Action<FruitData> OnFruitCollected;
    public static Action<JuiceData> OnJuiceProcessed;
    public static Action<JuiceData, int> OnJuiceSold;
}