using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI modularHardwareAccelerationText;
    [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI fullScreenText;

    [Header("References Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectSlider;

    private readonly Dictionary<bool, string> fullScreenTexts = new() {
        {true, "FullScreen"},
        {false, "Windowed"}
    };

    public void _Button_ModularHardwareAccelerationClicked() {
        SettingsManager.instance.modularHardwareAcceleration = !SettingsManager.instance.modularHardwareAcceleration;
    }

    public void _Button_FullScreenButtonClicked() {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private string GetDispText(bool from) { if (from) return "On"; else return "Off"; }

    private void Update() {
        SettingsManager.instance.musicVolume = (int)(musicVolumeSlider.value * 100f);
        SettingsManager.instance.soundEffectsVolume = (int)(soundEffectSlider.value * 100f);

        modularHardwareAccelerationText.text = GetDispText(SettingsManager.instance.modularHardwareAcceleration);
        soundEffectsVolumeText.text = $"Sound Effects Volume: {SettingsManager.instance.soundEffectsVolume}";
        musicVolumeText.text = $"Music Volume: {SettingsManager.instance.musicVolume}";
        fullScreenText.text = fullScreenTexts[Screen.fullScreen];
    }
}
