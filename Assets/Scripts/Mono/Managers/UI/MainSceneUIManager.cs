using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject dragger;
    public GameObject pauseMenu;

    [Header("References Resources")]
    [SerializeField] private TextMeshProUGUI resourceGoldText;

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
    private bool mouseBlocked = false;

    public bool IsPlacingObject() { return placingObject; }
    public void ObjectPlaced() { placingObject = null; }
    public SOPlaceableObject GetObjectToPlace() { return placingObject; }

    public void _Button_NextWaveButtonClicked() {
        if (!Utils.GetManager<WaveManager>().waveActive) Utils.GetManager<WaveManager>().StartWave();
    }

    public void _Button_PauseMenuQuitButtonClicked() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void _Button_PauseMenuResumeButtonClicked() {
        Utils.GetManager<RunManager>().Unpause();
    }

    public void _Button_PauseMenuSaveGameButtonClicked() {
        Debug.Log("Save Game");
    }

    public void _Button_EventMenuContinueButtonClicked() {
        Utils.GetManager<EventManager>().Event();
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

    private void Update() {
        dragger.SetActive(placingObject);
        dragger.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (placingObject) {
                PlaceInventoryItem(placingObject);
                placingObject = null;
            } else {
                if (Utils.GetManager<RunManager>().paused) {
                    Utils.GetManager<RunManager>().Unpause();
                } else {
                    Utils.GetManager<RunManager>().Pause();
                }
            }
        }

        if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.UIBlocksMouse)) {
            mouseBlocked = true;
        } else {
            mouseBlocked = false;
        }
        Debug.Log(mouseBlocked);

        SOEvent current_event = Utils.GetManager<EventManager>().currentEvent;
        eventMenu.SetActive(current_event);
        eventName.text = current_event?.displayName;
        eventDescription.text = current_event?.GetDescription();

        resourceGoldText.text = $"Gold: {GameManager.instance.Game.GetResources()[GameManager.Resources.Gold]}";
    }
}
