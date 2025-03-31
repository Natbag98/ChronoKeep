using UnityEngine;

[System.Serializable]
public class FactionData {
    public GameManager.FactionTypes factionType;
    public string name;
    public string rulerName;

    public FactionData(Faction faction) {
        factionType = faction.FactionType;
        name = faction.Name;
        rulerName = faction.Ruler;
    }

    public FactionData() {}
}
