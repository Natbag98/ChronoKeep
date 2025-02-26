using TMPro;
using UnityEngine;

public class SettingsUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI modularHardwareAccelerationText;

    public void _Button_ModularHardwareAccelerationClicked() {
        SettingsManager.instance.modularHardwareAcceleration = !SettingsManager.instance.modularHardwareAcceleration;
    }

    private string GetDispText(bool from) { if (from) return "On"; else return "Off"; }

    private void Update() {
        modularHardwareAccelerationText.text = GetDispText(SettingsManager.instance.modularHardwareAcceleration);
    }
}
