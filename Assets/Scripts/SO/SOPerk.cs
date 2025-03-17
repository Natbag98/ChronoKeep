using UnityEngine;

[CreateAssetMenu(fileName = "SOPerk", menuName = "SOPerk")]
public class SOPerk : ScriptableObject, IUnlockTrackable {
    [Header("Attributes")]
    public string displayName;
    public string description;
    public GameManager.PerkTrees tree;
    public int index;
    public SOPerk requiredPerk;

    [Header("Behaviour")]
    public Mod[] modsToApply;

    public bool Unlockable() {
        if (!GameManager.instance.Game.perksUnlockTracker.unlocked[this]) {
            if (requiredPerk == null || GameManager.instance.Game.perksUnlockTracker.unlocked[requiredPerk]) return true;
        }
        return false;
    }

    public GameObject GetPrefab() { return new(); }
}
