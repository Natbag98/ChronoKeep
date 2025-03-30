using UnityEngine;

[System.Serializable]
public class GameData {
    public bool runActive;
    public RunData runData = null;

    public string[][] baseTerrain;
    public UnlockTrackerData<SOPerk> perkUnlockTracker;
    public UnlockTrackerData<SOPlaceableObject> placeableObjectUnlockTracker;
}
