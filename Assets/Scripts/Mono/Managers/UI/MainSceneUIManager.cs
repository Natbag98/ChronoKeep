using UnityEngine;

public class MainSceneUIManager : MonoBehaviour {
    public static MainSceneUIManager instance;

    [Header("References")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject dragger;
    public GameObject pauseMenu;

    private SOPlaceableObject placingObject;

    public bool IsPlacingObject() { return placingObject; }
    public void ObjectPlaced() { placingObject = null; }

    public SOPlaceableObject GetObjectToPlace() {
        return placingObject;
    }

    public void _Button_NextWaveButtonClicked() {
        if (!WaveManager.instance.waveActive) WaveManager.instance.StartWave();
    }

    public void StartPlacing(SOPlaceableObject placeable_object) {
        placingObject = placeable_object;
    }

    /// <summary>
    /// Places a placeableObject in the players inventory.
    /// </summary>
    /// <param name="placeable_object">The object to place.</param>
    public void PlaceInventoryItem(SOPlaceableObject placeable_object) {
        InventoryItem item = Instantiate(inventoryItemPrefab, inventoryPanel).GetComponent<InventoryItem>();
        item.placeableObject = placeable_object;
    }

    private void Start() {
        instance = this;
    }

    private void Update() {
        dragger.SetActive(placingObject);
        dragger.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (placingObject) {
                PlaceInventoryItem(placingObject);
                placingObject = null;
            } else {
                if (RunManager.instance.paused) {
                    RunManager.instance.Unpause();
                } else {
                    RunManager.instance.Pause();
                }
            }
        }
    }
}
