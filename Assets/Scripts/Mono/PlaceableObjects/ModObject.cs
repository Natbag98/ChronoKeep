using System;
using System.Collections.Generic;
using UnityEngine;

public class ModObject : PlaceableObject {
    [Header("Mod Tower")]
    [SerializeField] private Mod[] modsToApply;

    [SerializeField] private List<PlaceableObject> objectsWithMod;

    protected override void Update() {
        base.Update();

        foreach (Plot plot in GetPlotsInRange()) {
            PlaceableObject placeable_object = plot.GetComponentInChildren<PlaceableObject>();
            if (placeable_object != null && !objectsWithMod.Contains(placeable_object)) {
                foreach (Mod mod in modsToApply) plot.GetComponentInChildren<PlaceableObject>()?.AddMod(mod);
                objectsWithMod.Add(placeable_object);
            }
        }
    }
}
