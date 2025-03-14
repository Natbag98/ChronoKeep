using System;
using UnityEngine;

public class ModObject : PlaceableObject {
    [Header("Mod Tower")]
    [SerializeField] private Mod[] modsToApply;

    private void WaveStart(object _, EventArgs __) {
        foreach (Plot plot in GetPlotsInRange()) {
            foreach (Mod mod in modsToApply) plot.GetComponentInChildren<PlaceableObject>()?.AddMod(mod);
        }
    }

    private void WaveEnd(object _, EventArgs __) {
        foreach (Plot plot in GetPlotsInRange()) {
            foreach (Mod mod in modsToApply) plot.GetComponentInChildren<PlaceableObject>()?.RemoveMod(mod);
        }
    }

    protected override void Start() {
        base.Start();
        Utils.GetManager<WaveManager>().waveStart += WaveStart;
        Utils.GetManager<WaveManager>().waveEnd += WaveEnd;
    }
}
