using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour {
    public static MainMenuUIManager instance;

    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    public void _Button_NewGameButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }

    public void _Button_LoadGameButtonClicked() {
        Debug.Log("Load Game");
    }

    public void _Button_SettingsButtonClicked() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void _Button_QuitGameButtonClicked() {
        Application.Quit();
    }

    public void _Button_SettingsMenuBackButtonClicked() {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Start() {
        instance = this;
    }
}
