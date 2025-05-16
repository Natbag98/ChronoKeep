using System;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour, ISaveSystem {
    public static TutorialManager instance;

    [Header("Referenes")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI nextButtonText;

    [Header("Attibutes")]
    [SerializeField] private string[] texts;

    private int tutorial = 0;
    private bool tutorialActive;

    public void _Button_TutorialBackButtonClicked() {
        tutorial = Math.Max(0, tutorial - 1);
    }

    public void _Button_TuorialNextButtonClicked() {
        if (tutorial + 1 == texts.Length) {
            tutorialActive = false;
            tutorialPanel.SetActive(false);
        }

        tutorial += 1;
    }

    public void StartTutorial() {
        tutorial = 0;
        tutorialActive = true;
        tutorialPanel.SetActive(true);
    }

    private void Update() {
        if (tutorialActive) {
            tutorialText.text = texts[tutorial];
            if (tutorial + 1 == texts.Length) {
                nextButtonText.text = "Finish";
            } else {
                nextButtonText.text = "Next";
            }
        }
    }

    private void Awake() {
        instance = this;
    }

    public void SaveData(GameData data) {
        data.tutorialActive = tutorialActive;
        data.tutorial = tutorial;
    }

    public void LoadData(GameData data) {
        if (tutorialActive) {
            StartTutorial();
            tutorial = data.tutorial;
        } else {
            tutorialActive = false;
            tutorialPanel.SetActive(false);
        }
    }
}
