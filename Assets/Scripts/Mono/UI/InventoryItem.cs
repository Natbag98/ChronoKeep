using UnityEngine;

public class InventoryItem : MonoBehaviour, ISaveSystem {
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

    public void SaveData(GameData data) {
        data.runData.inventoryItems.Add(placeableObject.name);
    }

    public void LoadData(GameData data) {}
}
