using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tag : MonoBehaviour {
    public enum Tags {
        UIBlocksMouse,
        Spawner,
        Tower,
        TowerViewer,
        PopulationCenter,
        PerkUI,
        CharacterViewer,
        UpgradeUI,
        Unit,
        Projectile
    }

    public List<Tags> tags;
    public bool HasTag(Tags tag) { return tags.Contains(tag); }
    public bool HasTag(Tags[] tags) { foreach (Tags tag in tags) if (!this.tags.Contains(tag)) return false; return true; }
}
