using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour, ISaveSystem {
    [HideInInspector] public SOPlaceableObject placeableObject;
    private TextMeshProUGUI text;

    public void _Button_InventoryItemClicked() {
        if (!WaveManager.instance.waveActive) {
            MainSceneUIManager.instance.StartPlacing(placeableObject);
            Destroy(gameObject);
        }
    }

    private void Start() {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Update() {
        GameManager.instance.Game.placeableObjectsUnlockTracker.UpdateDiscovered(placeableObject);
        text.text = placeableObject.displayName;
    }

    public void SaveData(GameData data) {
        data.runData.inventoryItems.Add(placeableObject.name);
    }

    public void LoadData(GameData data) {}
}
