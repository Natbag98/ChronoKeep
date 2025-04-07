using System;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour {
    public static PopupManager instance;

    [SerializeField] private int popupTime;
    [SerializeField] private GameObject popup;
    [SerializeField] private TextMeshProUGUI popupText;

    private float currentTime = 0;

    public void Display(String text) {
        popupText.text = text;
        popup.SetActive(true);
        currentTime = 0;
    }

    private void Start() {
        instance = this;
    }

    private void Update() {
        currentTime += Time.deltaTime;
        if (currentTime > popupTime) {
            popup.SetActive(false);
        }
    }
}
