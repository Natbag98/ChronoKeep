using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceObject", menuName = "Event/PlaceObject")]
public class PlaceObject : SOEvent {
    [Header("PlaceInventoryObject")]
    [SerializeField] Tag.Tags[] potentialObjectTags;
    [SerializeField] GameManager.FactionTypes factionType;

    private SOPlaceableObject objectToPlace;
    private Faction faction;

    public override string GetDescription() {
        return $"A {objectToPlace.displayName} of {faction.Name} will be placed on the map";
    }

    public override void Setup() {
        objectToPlace = GameManager.instance.Game.placeableObjectsUnlockTracker.GetRandomUnlocked(potentialObjectTags);
        List<Faction> potential_factions = new();
        foreach (Faction faction in Utils.GetManager<RunManager>().factions) if (faction.FactionType == factionType) potential_factions.Add(faction);
        faction = Utils.Choice(potential_factions);
    }

    public override void Event() {
        Utils.GetManager<RunManager>().PlaceRandomObject(objectToPlace, faction, factionType == GameManager.FactionTypes.BarbarianClan);
    }
}
