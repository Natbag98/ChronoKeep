using UnityEngine;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager instance;

    [Header("Default Values")]
    public bool modularHardwareAcceleration = false;
    [HideInInspector] public int soundEffectsVolume = 100;
    [HideInInspector] public int musicVolume = 100;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
