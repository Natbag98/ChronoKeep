using UnityEngine;

[CreateAssetMenu(fileName = "PlaceInventoryObject", menuName = "Event/PlaceInventoryObject")]
public class PlaceInventoryObject : SOEvent {
    [Header("PlaceInventoryObject")]
    [SerializeField] Tag.Tags[] potentialObjectTags;

    private SOPlaceableObject objectToPlace;

    public override string GetDescription() {
        return $"A {objectToPlace.displayName} will be placed in your inventory";
    }

    public override void Setup() {
        objectToPlace = GameManager.instance.Game.placeableObjectsUnlockTracker.GetRandomUnlocked(potentialObjectTags);
    }

    public override void Event() {
        MainSceneUIManager.instance.PlaceInventoryItem(objectToPlace);
    }
}
