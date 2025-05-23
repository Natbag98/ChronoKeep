using System;
using System.Collections.Generic;
using UnityEngine;

public class ModObject : PlaceableObject {
    [Header("Mod Tower")]
    [SerializeField] private Mod[] modsToApply;

    protected override void Update() {
        base.Update();

        foreach (Plot plot in GetPlotsInRange()) {
            foreach (Mod mod in modsToApply) plot.GetComponentInChildren<PlaceableObject>()?.AddMod(mod);
        }
    }
}
