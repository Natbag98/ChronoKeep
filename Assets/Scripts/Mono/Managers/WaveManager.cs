using System;
using UnityEngine;

public class WaveManager : MonoBehaviour, ISaveSystem {
    public static WaveManager instance;

    private int wave = 0;
    [HideInInspector] public bool waveActive = false;

    [HideInInspector] public int hostileWaveSpawners;
    [HideInInspector] public int hostileWaveSpawnersFinished;

    public event EventHandler waveStart;
    public event EventHandler waveEnd;

    public void StartWave() {
        waveActive = true;
        wave++;
        waveStart?.Invoke(null, EventArgs.Empty);

        hostileWaveSpawners = hostileWaveSpawnersFinished = 0;

        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            faction.OnWaveStart(wave);
        }
    }

    void Update() {
        if (waveActive) {
            if (
                hostileWaveSpawners == hostileWaveSpawnersFinished &&
                RunManager.instance.characterContainer.childCount < 1
            ) {
                waveEnd?.Invoke(null, EventArgs.Empty);
                waveActive = false;
            }
        }
    }

    private void Start() {
        instance = this;
    }

    public void SaveData(GameData data) {
        data.runData.wave = wave;
    }

    public void LoadData(GameData data) {
        wave = data.runData.wave;
    }
}
