using System;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    public static WaveManager instance;

    public class OnWaveStartEventArgs : EventArgs {
        public int wavePower;
    }

    public event EventHandler<OnWaveStartEventArgs> onWaveStart;

    private int wave = 0;

    public void StartWave() {
        wave++;
        onWaveStart.Invoke(null, new OnWaveStartEventArgs() { wavePower = wave * 10 });
    }

    private void Start() {
        instance = this;
    }
}
