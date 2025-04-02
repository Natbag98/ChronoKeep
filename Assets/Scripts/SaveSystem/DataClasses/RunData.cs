using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunData {
    public List<Mod> globalMods;
    public List<PlaceableObjectData> placeableObjects = new();

    public RunData() {}
}
