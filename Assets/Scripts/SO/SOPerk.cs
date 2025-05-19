using UnityEngine;

[CreateAssetMenu(fileName = "SOPerk", menuName = "SOPerk")]
public class SOPerk : ScriptableObject, IUnlockTrackable {
    [Header("Attributes")]
    public string displayName;
    public string description;
    public GameManager.PerkTrees tree;
    public int index;
    public SOPerk requiredPerk;
    public int cost;

    [Header("Behaviour")]
    public float scoreMultIncrease;
    public float skillMultIncrease;
    public Mod[] modsToApply;

    public bool Unlockable() {
        if (!GameManager.instance.Game.perksUnlockTracker.unlocked[this]) {
            if (requiredPerk == null || GameManager.instance.Game.perksUnlockTracker.unlocked[requiredPerk]) {
                return GameManager.instance.Game.skillPoints >= cost;
            }
        }
        return false;
    }

    public GameObject GetPrefab() { return new(); }
}
