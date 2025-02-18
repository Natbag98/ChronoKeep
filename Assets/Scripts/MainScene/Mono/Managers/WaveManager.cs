using System;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance;


    private int wave = 0;
    [HideInInspector] public bool waveActive = false;

    public void StartWave() {
        waveActive = true;
        wave++;
        foreach (Faction faction in RunManager.instance.factions) {
            faction.OnWaveStart(wave);
        }
    }

    private void Start() {
        instance = this;
    }
}
