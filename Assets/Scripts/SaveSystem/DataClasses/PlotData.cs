using UnityEngine;

[System.Serializable]
public class PlotData {
    public string faction;
    public string plotSO;
    public PlaceableObjectData placedObject = null;
    public bool visiblToPlayer;

    public PlotData() {}
}
