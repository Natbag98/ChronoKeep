using System;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    private int wave = 0;
    [HideInInspector] public bool waveActive = false;

    [HideInInspector] public int hostileWaveSpawners;
    [HideInInspector] public int hostileWaveSpawnersFinished;

    public event EventHandler waveEnd;

    public void StartWave() {
        waveActive = true;
        wave++;

        hostileWaveSpawners = hostileWaveSpawnersFinished = 0;

        foreach (Faction faction in Utils.GetManager<RunManager>().factions) {
            faction.OnWaveStart(wave);
        }
    }

    void Update() {
        if (waveActive) {
            if (
                hostileWaveSpawners == hostileWaveSpawnersFinished &&
                Utils.GetManager<RunManager>().characterContainer.childCount < 1
            ) {
                waveEnd?.Invoke(null, EventArgs.Empty);
                waveActive = false;
            }
        }
    }
}
