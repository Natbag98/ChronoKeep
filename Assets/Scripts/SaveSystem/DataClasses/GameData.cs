using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {
    public bool runActive;
    public RunData runData = null;

    public string[][] baseTerrain;
    public UnlockTrackerData<SOPerk> perkUnlockTracker;
    public UnlockTrackerData<SOPlaceableObject> placeableObjectUnlockTracker;
    public List<FactionData> factionData = new();
    public FactionData playerFaction;
    public List<BaseObjectInfoData> baseObjectInfo;
}
