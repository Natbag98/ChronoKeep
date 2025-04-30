using System;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour, ISaveSystem {
    public static WaveManager instance;

    private int wave = 0;
    [HideInInspector] public bool waveActive = false;
    private float secondTimer = 0;

    public event EventHandler waveStart;
    public event EventHandler waveEnd;

    public int GetWave() { return wave; }

    public void StartWave() {
        secondTimer = 0;
        waveActive = true;
        wave++;
        waveStart?.Invoke(null, EventArgs.Empty);

        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            faction.OnWaveStart(wave);
        }
    }

    void Update() {
        if (waveActive) {
            secondTimer += Time.deltaTime;
            if (secondTimer > 1) {
                secondTimer = 0;

                foreach (Spawner spawner in FindObjectsByType<Spawner>(FindObjectsSortMode.None)) {
                    if (spawner.partOfHostileWave && spawner.spawning) return;
                }

                if (
                    RunManager.instance.characterContainer.childCount < 1
                ) {
                    RunManager.instance.AddScore(100);
                    GameManager.instance.Game.ResetManpower();
                    waveEnd?.Invoke(null, EventArgs.Empty);
                    waveActive = false;
                }
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
