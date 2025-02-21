using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour {
    public static MainMenuUIManager instance;

    public void _Button_NewGameButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }

    public void _Button_LoadGameButtonClicked() {
        Debug.Log("Load Game");
    }

    public void _Button_SettingsButtonClicked() {
        Debug.Log("Settings");
    }

    public void _Button_QuitGameButtonClicked() {
        Application.Quit();
    }

    private void Start() {
        instance = this;
    }
}
