using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIManager : MonoBehaviour, ISaveSystem {
    public static MainSceneUIManager instance;

    [Header("References")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject dragger;
    public GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI speedText;
    public GameObject enemyDotPrefab;

    [Header("References Resources")]
    [SerializeField] private TextMeshProUGUI resourceGoldText;
    [SerializeField] private TextMeshProUGUI resourceManPowerText;
    [SerializeField] private TextMeshProUGUI resoureStoneText;

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
    public GameObject factionTextObject;
    public TextMeshProUGUI factionText;
    public GameObject placePlotInfoPanel;
    public TextMeshProUGUI placePlotInfoPanelText;

    [Header("References Upgrade Menu")]
    [SerializeField] private GameObject upgradePrefab;
    public GameObject upgradePanel;
    [SerializeField] private Transform upgradesHolder;
    [SerializeField] private TextMeshProUGUI upgradeText;

    [Header("References Trader UI")]
    [SerializeField] private GameObject traderPanel;
    [SerializeField] private Transform shopItemHolder;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private TextMeshProUGUI shopDesc;

    [Header("References Diplomacy Panel")]
    [SerializeField] private GameObject diploPanel;
    [SerializeField] private TextMeshProUGUI diploTitle;
    [SerializeField] private TextMeshProUGUI diploFlavour;
    [SerializeField] private GameObject diploButtons;
    [SerializeField] private GameObject diploNoOptions;
    [SerializeField] private TextMeshProUGUI diploOptionDesc;
    [SerializeField] private GameObject diploWarButton;
    [SerializeField] private GameObject diploPeaceButton;
    [SerializeField] private GameObject diploEnvoyButton;

    private SOPlaceableObject placingObject;
    [HideInInspector] public bool mouseBlocked = false;
    [HideInInspector] public Plot upgradePlot;
    private Dictionary<GameManager.Resources, int> resourcesPerWave;
    private Dictionary<GameManager.Resources, TextMeshProUGUI> text_dict;
    [HideInInspector] public bool shopActive;
    private Faction currentFaction;

    public event EventHandler resetUpgrades;

    public bool IsPlacingObject() { return placingObject; }
    public void ObjectPlaced() { placingObject = null; }
    public SOPlaceableObject GetObjectToPlace() { return placingObject; }

    public void _Button_NextWaveButtonClicked() {
        if (
            upgradePanel.activeSelf ||
            RunManager.instance.paused ||
            MainSceneUIManager.instance.IsPlacingObject()
        ) return;
        if (!WaveManager.instance.waveActive) {
            WaveManager.instance.StartWave(); 
        } else {
            PopupManager.instance.Display("Wave already active");
        }
    }

    public void _Button_PauseMenuSaveAndQuitButtonClicked() {
        if (WaveManager.instance.waveActive) {
            PopupManager.instance.Display("Cannot save while a wave is active");
        } else {
            SaveSystemManager.instance.SaveGame();
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void _Button_PauseMenuEndRunClicked() {
        RunManager.instance.GameOver();
    }

    public void _Button_PauseMenuResumeButtonClicked() {
        RunManager.instance.Unpause();
    }

    public void _Button_EventMenuContinueButtonClicked() {
        EventManager.instance.Event();
    }

    public void _Button_PauseButtonClicked() {
        if (RunManager.instance.paused) return;
        RunManager.instance.Pause();
    }

    public void _Button_GameSpeedDownButtonClicked() {
        if (RunManager.instance.paused) return;
        RunManager.instance.simSpeed = Math.Max(GameManager.instance.minGameSpeed, RunManager.instance.simSpeed - 0.5f);
    }

    public void _Button_GameSpeedUpButtonClicked() {
        if (RunManager.instance.paused) return;
        RunManager.instance.simSpeed = Math.Min(GameManager.instance.maxGameSpeed, RunManager.instance.simSpeed + 0.5f);
    }

    public void _Button_TraderContinueButtonClicked() {
        traderPanel.SetActive(false);
    }

    public void _Button_DeclareWarButtonClicked() {
        GameManager.instance.Game.PlayerFaction.DeclareWar(currentFaction);
        InitializeDiploMenu(currentFaction);
    }

    public void _Button_PeaceButtonClicked() {
        if (GameManager.instance.Game.SpendResources(new() {{GameManager.Resources.Gold, currentFaction.peaceCost}})) {
            currentFaction.peaceCost += GameManager.Random.Next(5, 10);
            currentFaction.MakePeace(GameManager.instance.Game.PlayerFaction);
            InitializeDiploMenu(currentFaction);
        }
    }

    public void _Button_EnvoyButtonClicked() {
        if (GameManager.instance.Game.SpendResources(new() {{GameManager.Resources.Gold, currentFaction.envoyCost}})) {
            foreach (Plot plot in RunManager.instance.GetAllFactionPlots(currentFaction)) {
                foreach (Plot neighbour in plot.GetNeighbours(square: true, include_self: true)) neighbour.SetVisibleToPlayer(true);
            }
            InitializeDiploMenu(currentFaction);
        }
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

    public void InitializeUpgradesMenu(Plot plot) {
        CameraSystem.instance.cameraBlocked = true;
        mouseBlocked = true;
        upgradePlot = plot;
        upgradePanel.SetActive(true);
        foreach (SOUpgrade upgrade in Utils.GetAllAssets<SOUpgrade>()) {
            if (upgrade.IsAvailable(plot.GetComponentInChildren<PlaceableObject>())) {
                Upgrade new_upgrade = Instantiate(
                    upgradePrefab,
                    upgradesHolder
                ).GetComponent<Upgrade>();
                new_upgrade.upgrade = upgrade;
            }
        }
    }

    public void InitializeDiploMenu(Faction faction) {
        CameraSystem.instance.cameraBlocked = true;
        mouseBlocked = true;
        diploPanel.SetActive(true);
        diploNoOptions.SetActive(false);
        diploButtons.SetActive(true);

        if (!GameManager.instance.Game.PlayerFaction.atWarWith[faction]) {
            diploWarButton.SetActive(true);
        } else {
            diploWarButton.SetActive(false);
        }

        if (GameManager.instance.Game.PlayerFaction.atWarWith[faction]) {
            diploPeaceButton.SetActive(true);
        } else {
            diploPeaceButton.SetActive(false);
        }

        diploEnvoyButton.SetActive(false);
        if (!GameManager.instance.Game.PlayerFaction.atWarWith[faction]) {
            foreach (Plot plot in RunManager.instance.GetAllPlotsWithFactionObjects(currentFaction)) {
                if (plot.placedObjectSO.objectType == GameManager.PlaceableObjectTypes.Castle && !plot.visibleToPlayer) diploEnvoyButton.SetActive(true);
            }
        }

        diploTitle.text = faction.Name;
        diploFlavour.text = $"King {faction.Ruler} has granted your diplomats an audience";
        if (GameManager.instance.Game.PlayerFaction.atWarWith[faction]) diploFlavour.text += ", despite the ongoing war being your kingdoms";
        currentFaction = faction;
    }

    private void Start() {
        instance = this;
        text_dict = new() {
            {GameManager.Resources.Gold, resourceGoldText},
            {GameManager.Resources.ManPower, resourceManPowerText},
            {GameManager.Resources.Stone, resoureStoneText}
        };
    }

    public void UpdateResourceGain() {
        resourcesPerWave = new();
        foreach (var resource in Enum.GetValues(typeof(GameManager.Resources))) {
            resourcesPerWave.Add((GameManager.Resources)resource, 0);
        }
        foreach (Plot plot in RunManager.instance.GetAllPlotsWithFactionObjects(GameManager.instance.Game.PlayerFaction)) {
            foreach (var resource in plot.GetComponentInChildren<PlaceableObject>().GetResourcesPerWave()) {
                resourcesPerWave[resource.Key] += resource.Value;
            }
        }
    }

    private void Update() {
        if (resourcesPerWave == null) UpdateResourceGain();
        dragger.SetActive(placingObject);
        dragger.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (placingObject) {
                PlaceInventoryItem(placingObject);
                placingObject = null;
            } else if (upgradePanel.activeSelf) {
                CameraSystem.instance.cameraBlocked = false;
                resetUpgrades?.Invoke(this, EventArgs.Empty);
                upgradePanel.SetActive(false);
                mouseBlocked = false;
            } else if (diploPanel.activeSelf) {
                diploPanel.SetActive(false);
                CameraSystem.instance.cameraBlocked = false;
                mouseBlocked = false;
            } else {
                if (RunManager.instance.paused) {
                    RunManager.instance.Unpause();
                } else {
                    RunManager.instance.Pause();
                }
            }
        }

        if (upgradePanel.activeSelf) {
            GameObject hover = Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.UpgradeUI);
            if (hover != null) {
                Upgrade upgrade = hover.GetComponent<Upgrade>();
                string cost_text = "";
                foreach (var cost in upgrade.upgrade.cost.GetDict()) {
                    cost_text += $"{cost.Key}: {cost.Value}\n";
                }
                string buy_text = "";
                if (GameManager.instance.Game.CanSpendResources(upgrade.upgrade.cost.GetDict())) buy_text = "Click to Buy";
                upgradeText.text = $"{upgrade.upgrade.displayName}\n\n{upgrade.upgrade.description}\n\nCost:\n{cost_text}\n{buy_text}";
            } else {
                upgradeText.text = "";
            }
        }

        if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.UIBlocksMouse) || RunManager.instance.paused) {
            mouseBlocked = true;
        } else {
            if (!upgradePanel.activeSelf) mouseBlocked = false;
        }

        SOEvent current_event = EventManager.instance.currentEvent;
        eventMenu.SetActive(current_event);
        eventName.text = current_event?.displayName;
        eventDescription.text = current_event?.GetDescription();

        foreach (var r in Enum.GetValues(typeof(GameManager.Resources))) {
            GameManager.Resources resource = (GameManager.Resources)r;
            string add_text = "";
            if (resourcesPerWave[resource] > 0) add_text = $" + {resourcesPerWave[resource]}";
            text_dict[resource].text = $"{resource}: {GameManager.instance.Game.GetResources()[resource]}{add_text}";
        }

        if (!RunManager.instance.paused) speedText.text = $"{RunManager.instance.simSpeed}x";

        if (current_event == null && shopActive) {
            traderPanel.SetActive(true);
            if (transform.childCount > 0) foreach (Transform shop_item in shopItemHolder) Destroy(shop_item.gameObject);
            for (int i = 0; i < GameManager.Random.Next(2, 5); i++) Instantiate(shopItemPrefab, shopItemHolder);
            shopActive = false;
        }

        if (traderPanel.activeSelf) {
            ShopItem item = Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.ShopItem)?.GetComponent<ShopItem>();
            if (item != null) shopDesc.text = item.objectToBuy.description; else shopDesc.text = "";
        } else {
           shopDesc.text = ""; 
        }

        if (diploPanel.activeSelf) {
            if (
                !diploWarButton.activeSelf &&
                !diploPeaceButton.activeSelf &&
                !diploEnvoyButton.activeSelf
            ) {
                diploNoOptions.SetActive(true);
                diploButtons.SetActive(false);
            }

            GameObject button = Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.DiploOption);
            if (button != null) {
                if (button.name == "DeclareWarButton") diploOptionDesc.text = "Declare war on this faction";
                if (button.name == "MakePeaceButton") diploOptionDesc.text = $"{currentFaction.Ruler} will make peace with you for a price of {currentFaction.peaceCost} gold";
                if (button.name == "EnvoyButton") diploOptionDesc.text = $"{currentFaction.Ruler} will recieve you envoy, which will cost you {currentFaction.envoyCost} gold to send (this will reveal their territory to you)";
            } else {
                diploOptionDesc.text = "";
            }
        }

        InventoryItem inventory_item = Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.InventoryItem)?.GetComponent<InventoryItem>();
        if (inventory_item != null) {
            placePlotInfoPanel.SetActive(true);
            placePlotInfoPanelText.text = $"{inventory_item.placeableObject.description}\nCost:";
            foreach (var cost in inventory_item.placeableObject.placementCost.GetDict()) {
                placePlotInfoPanelText.text += $"\n{cost.Key}: {cost.Value}";
            }
        } else {
            placePlotInfoPanel.SetActive(false);
        }
    }

    public void SaveData(GameData data) {}

    public void LoadData(GameData data) {
        foreach (Transform child in inventoryPanel) Destroy(child.gameObject);
        foreach (string item in data.runData.inventoryItems) PlaceInventoryItem(Utils.GetAsset<SOPlaceableObject>(item));
    }
}
