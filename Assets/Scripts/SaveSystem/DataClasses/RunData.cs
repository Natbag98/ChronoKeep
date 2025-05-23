using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunData {
    public int wave;
    public float eventChance;
    public List<Mod> globalMods;
    public PlotData[][] plotData;
    public Dictionary<string, Dictionary<string, bool>> factionsWars = new();
    public List<string> inventoryItems = new();
    public int score;

    public RunData() {}
}
