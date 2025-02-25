using UnityEngine;

[CreateAssetMenu(fileName = "PlaceInventoryObject", menuName = "Event/PlaceInventoryObject")]
public class PlaceInventoryObject : SOEvent {
    [Header("PlaceInventoryObject")]
    [SerializeField] SOPlaceableObject[] potentialObjects;

    private SOPlaceableObject objectToPlace;

    public override string GetDescription() {
        return $"A {objectToPlace.name} will be placed in your inventory";
    }

    public override void Setup() {
        objectToPlace = Utils.Choice(potentialObjects);
    }

    public override void Event() {
        Utils.GetManager<MainSceneUIManager>().PlaceInventoryItem(objectToPlace);
    }
}
