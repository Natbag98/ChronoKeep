using UnityEngine;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager instance;

    [Header("Default Values")]
    public bool modularHardwareAcceleration = false;
    [Range(0, 100)] public int soundEffectsVolume = 100;
    [Range(0, 100)] public int musicVolume = 100;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
    }
}
