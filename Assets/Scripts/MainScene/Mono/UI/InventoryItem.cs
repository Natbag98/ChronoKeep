using UnityEngine;

public class InventoryItem : MonoBehaviour {
    [HideInInspector] public SOPlaceableObject placeableObject;

    public void _Button_InventoryItemClicked() {
        MainSceneUIManager.instance.StartPlacing(placeableObject);
    }
}
