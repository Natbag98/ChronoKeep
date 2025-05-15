using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneUIManager : MonoBehaviour {
    public static CutSceneUIManager instance;
    private readonly List<string> text = new() {
        "Your father’s rule left the kingdom in ruins. As the new prince, the burden is yours to bear. Enemies close in from all sides—defend your castle, protect your people, and try to survive. The fate of the realm lies in your hands.",
        "The kingdom has fallen... but the Council watches. Time unravels, and you are given another chance. With each failure, you grow stronger. Learn. Adapt. Defend. The past will haunt you—until you change it."
    };

    [Header("References")]
    [SerializeField] private TextMeshProUGUI storyText;

    public void _Button_ContinueButtonClicked() {
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    private void Start() {
        instance = this;
        storyText.text = text[GameManager.instance.storyDisplay];
    }
}
