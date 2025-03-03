using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour {
    public void _Button_StartRunButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }

    public void _Button_SaveAndQuitButtonClicked() {
        SceneManager.LoadScene("MainMenuScene");
    }
}
