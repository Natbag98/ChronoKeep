using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject characterPerkMenu;

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
}
