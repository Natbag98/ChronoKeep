using UnityEngine;

public class MainSceneUIManager : MonoBehaviour {
    public static MainSceneUIManager instance;

    [Header("References")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;

    [SerializeField] private GameObject dragger;

    private SOPlaceableObject placingObject;

    public bool IsPlacingObject() { return placingObject; }
    public void ObjectPlaced() { placingObject = null; }

    public SOPlaceableObject GetObjectToPlace() {
        return placingObject;
    }

    public void StartPlacing(SOPlaceableObject placeable_object) {
        placingObject = placeable_object;
    }

    public void PlaceInventoryItem(SOPlaceableObject placeable_object) {
        InventoryItem item = Instantiate(inventoryItemPrefab, inventoryPanel).GetComponent<InventoryItem>();
        item.placeableObject = placeable_object;
    }

    private void Start() {
        instance = this;
    }
}
