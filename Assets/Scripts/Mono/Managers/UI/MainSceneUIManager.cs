using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIManager : MonoBehaviour {
    public static MainSceneUIManager instance;

    [Header("References")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject dragger;
    public GameObject pauseMenu;

    [Header("References Resources")]
    [SerializeField] private TextMeshProUGUI resourceGoldText;
    [SerializeField] private TextMeshProUGUI resourceManPowerText;

    [Header("References Event Menu")]
    [SerializeField] private GameObject eventMenu;
    [SerializeField] private TextMeshProUGUI eventName;
    [SerializeField] private TextMeshProUGUI eventDescription;

    [Header("References Info Panels")]
    public GameObject plotInfoPanel;
    public TextMeshProUGUI plotInfoName;
    public TextMeshProUGUI plotInfoDescription;
    public GameObject objectInfoPanel;
    public TextMeshProUGUI objectInfoName;
    public TextMeshProUGUI objectInfoDescription;

    private SOPlaceableObject placingObject;
    [HideInInspector] public bool mouseBlocked = false;

    public bool IsPlacingObject() { return placingObject; }
    public void ObjectPlaced() { placingObject = null; }
    public SOPlaceableObject GetObjectToPlace() { return placingObject; }

    public void _Button_NextWaveButtonClicked() {
        if (!WaveManager.instance.waveActive) WaveManager.instance.StartWave();
    }

    public void _Button_PauseMenuQuitButtonClicked() {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    public void _Button_PauseMenuResumeButtonClicked() {
        RunManager.instance.Unpause();
    }

    public void _Button_PauseMenuSaveGameButtonClicked() {
        Debug.Log("Save Game");
    }

    public void _Button_EventMenuContinueButtonClicked() {
        EventManager.instance.Event();
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

        if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.UIBlocksMouse) || RunManager.instance.paused) {
            mouseBlocked = true;
        } else {
            mouseBlocked = false;
        }

        SOEvent current_event = EventManager.instance.currentEvent;
        eventMenu.SetActive(current_event);
        eventName.text = current_event?.displayName;
        eventDescription.text = current_event?.GetDescription();

        resourceGoldText.text = $"Gold: {GameManager.instance.Game.GetResources()[GameManager.Resources.Gold]}";
        resourceManPowerText.text = $"Man Power: {GameManager.instance.Game.GetResources()[GameManager.Resources.ManPower]}";
    }
}
