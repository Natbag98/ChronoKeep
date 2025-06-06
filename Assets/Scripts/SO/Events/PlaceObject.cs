using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceObject", menuName = "Event/PlaceObject")]
public class PlaceObject : SOEvent {
    [Header("PlaceObject")]
    [SerializeField] private Tag.Tags[] potentialObjectTags;
    [SerializeField] private GameManager.FactionTypes factionType;
    [SerializeField] private bool onVisible;

    private SOPlaceableObject objectToPlace;
    private Faction faction;

    public override string GetDescription() {
        return $"A {objectToPlace.displayName} of {faction.Name} will be placed on the map";
    }

    public override void Setup() {
        objectToPlace = GameManager.instance.Game.placeableObjectsUnlockTracker.GetRandomUnlocked(potentialObjectTags);
        List<Faction> potential_factions = new();
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) if (faction.FactionType == factionType) potential_factions.Add(faction);
        faction = Utils.Choice(potential_factions);
    }

    public override void Event() {
        RunManager.instance.PlaceRandomObject(objectToPlace, faction, factionType == GameManager.FactionTypes.BarbarianClan, onVisible);
    }
}
