using System;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance;


    private int wave = 0;

    public void StartWave() {
        wave++;
        foreach (Faction faction in RunManager.instance.factions) {
            faction.OnWaveStart(wave);
        }
    }

    private void Start() {
        instance = this;
    }
}
