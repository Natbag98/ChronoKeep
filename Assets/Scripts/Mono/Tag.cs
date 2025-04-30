using System.Linq;
using UnityEngine;

public class Tag : MonoBehaviour {
    public enum Tags {
        UIBlocksMouse,
        Spawner,
        Tower,
        TowerViewer,
        PopulationCenter,
        PerkUI
    }

    public Tags[] tags;
    public bool HasTag(Tags tag) { return tags.Contains(tag); }
}
