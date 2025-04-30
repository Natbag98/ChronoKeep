using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour {
    public static GameSceneUIManager instance;

    [Header("References")]
    [SerializeField] private GameObject mainMenu;

    [Header("References Tower Viewer")]
    [SerializeField] private GameObject towerMenu;
    [SerializeField] private Transform towerHolder;
    [SerializeField] private GameObject towerViewerPrefab;
    public GameObject towerInfo;
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI towerDescription;

    [Header("References Character Viewer")]
    [SerializeField] private GameObject characterMenu;
    [SerializeField] private Transform characterHolder;
    [SerializeField] private GameObject characterViewerPrefab;
    public GameObject characterInfo;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterDescription;

    [Header("References Character Perks")]
    [SerializeField] private GameObject characterPerkMenu;
    [SerializeField] private Transform characterPerksHolder;
    [SerializeField] private TextMeshProUGUI perkDescriptionText;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private GameObject perkTreePrefab;
    [SerializeField] private GameObject perkPrefab;

    public void _Button_StartRunButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }

    public void _Button_SaveAndQuitButtonClicked() {
        SaveSystemManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void _Button_CharacterPerkButtonClicked() {
        mainMenu.SetActive(false);
        characterPerkMenu.SetActive(true);
    }

    public void _Button_ViewTowersButtonClicked() {
        mainMenu.SetActive(false);
        towerMenu.SetActive(true);
    }

    public void _Button_ViewTowersBackButtonClicked() {
        towerMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void _Button_CharacterPerksBackButtonClicked() {
        characterPerkMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void _Button_CharacterViewerButtonClicked() {
        mainMenu.SetActive(false);
        characterMenu.SetActive(true);
    }

    public void _Button_CharacterViewerBackButtonClicked() {
        characterMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Start() {
        instance = this;

        foreach (SOPlaceableObject placeable_object in Utils.GetAllAssets<SOPlaceableObject>()) {
            TowerViewer tower_viewer = Instantiate(towerViewerPrefab, towerHolder).GetComponent<TowerViewer>();
            tower_viewer.placeableObject = placeable_object;
        }

        foreach (SOCharacter character in Utils.GetAllAssets<SOCharacter>()) {
            CharacterViewer character_viewer = Instantiate(characterViewerPrefab, characterHolder).GetComponent<CharacterViewer>();
            character_viewer.character = character;
        }

        foreach (GameManager.PerkTrees tree in Utils.GetEnumValues<GameManager.PerkTrees>()) {
            Transform tree_transform = Instantiate(perkTreePrefab, characterPerksHolder).transform;
            foreach (SOPerk perk in Utils.GetAllAssets<SOPerk>()) {
                if (perk.tree == tree) {
                    Perk perk_object = Instantiate(perkPrefab, tree_transform).GetComponent<Perk>();
                    perk_object.transform.SetSiblingIndex(perk.index);
                    perk_object.perk = perk;
                }
            }
        }
    }

    private void Update() {
        if (!Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.TowerViewer)) {
            towerInfo.SetActive(false);
        }

        if (!Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.CharacterViewer)) {
            characterInfo.SetActive(false);
        }

        GameObject hover = Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.PerkUI);
        if (hover) {
            perkDescriptionText.text = hover.GetComponent<Perk>().perk.description;
            perkDescriptionText.text += $"\n\nCost: {hover.GetComponent<Perk>().perk.cost}";
            if (hover.GetComponent<Perk>().Unlockable()) {
                perkDescriptionText.text += "\nClick to Unlock";
            }
        } else {
            perkDescriptionText.text = "";
        }

        skillPointsText.text = $"Skill Points: {GameManager.instance.Game.skillPoints}";
    }
}
