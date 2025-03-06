using UnityEngine;

public class InventoryItem : MonoBehaviour {
    [HideInInspector] public SOPlaceableObject placeableObject;

    public void _Button_InventoryItemClicked() {
        if (!Utils.GetManager<WaveManager>().waveActive) {
            Utils.GetManager<MainSceneUIManager>().StartPlacing(placeableObject);
            Destroy(gameObject);
        }
    }

    public void Update() {
        GameManager.instance.Game.UpdateObjectsDiscovered(placeableObject);
    }
}
