using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject mainMenu;

    [Header("References Character Perks")]
    [SerializeField] private GameObject characterPerkMenu;
    [SerializeField] private Transform characterPerksHolder;
    [SerializeField] private GameObject perkTreePrefab;
    [SerializeField] private GameObject perkPrefab;

    public void _Button_StartRunButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }

    public void _Button_SaveAndQuitButtonClicked() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void _Button_CharacterPerkButtonClicked() {
        mainMenu.SetActive(false);
        characterPerkMenu.SetActive(true);
    }

    public void _Button_ViewTowersButtonClicked() {
        Debug.Log("View Towers");
    }

    public void _Button_CharacterPerksBackButtonClicked() {
        characterPerkMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Start() {
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
}
