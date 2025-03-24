using UnityEngine;

public class InventoryItem : MonoBehaviour {
    [HideInInspector] public SOPlaceableObject placeableObject;

    public void _Button_InventoryItemClicked() {
        if (!WaveManager.instance.waveActive) {
            MainSceneUIManager.instance.StartPlacing(placeableObject);
            Destroy(gameObject);
        }
    }

    public void Update() {
        GameManager.instance.Game.placeableObjectsUnlockTracker.UpdateDiscovered(placeableObject);
    }
}
