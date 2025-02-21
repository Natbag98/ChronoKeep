using UnityEngine;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager instance;

    [Header("Default Values")]
    public bool modularHardwareAcceleration = false;

    private void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}