using System;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance;

    private int wave = 0;
    [HideInInspector] public bool waveActive = false;

    [HideInInspector] public int hostileWaveSpawners;
    [HideInInspector] public int hostileWaveSpawnersFinished;

    public void StartWave() {
        waveActive = true;
        wave++;

        hostileWaveSpawners = hostileWaveSpawnersFinished = 0;

        foreach (Faction faction in RunManager.instance.factions) {
            faction.OnWaveStart(wave);
        }
    }

    private void Start() {
        instance = this;
    }

    void Update() {
        Debug.Log(waveActive);
        if (waveActive) {
            if (hostileWaveSpawners == hostileWaveSpawnersFinished) {
                waveActive = false;
            }
        }
    }
}
