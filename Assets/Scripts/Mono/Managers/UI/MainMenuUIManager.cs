using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour {
    [Header("References Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject startMenu;

    [Header("References Start Menu")]
    [SerializeField] TMP_InputField playerName;
    [SerializeField] TMP_InputField kingdomName;

    public void _Button_NewGameButtonClicked() {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
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

    public void _Button_StartMenuBackButtonClicked() {
        startMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void _Button_StartMenuStartButtonClicked() {
        GameManager.instance.playerName = playerName.text;
        GameManager.instance.kingdomName = kingdomName.text;
        GameManager.instance.NewGame();
        
        SceneManager.LoadScene("MainScene");
    }
}
