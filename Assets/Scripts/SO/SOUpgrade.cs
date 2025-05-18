using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SOUpgrade", menuName = "SOUpgrade")]
public class SOUpgrade : ScriptableObject {
    [Header("UI")]
    public string displayName;
    public string description;

    [Header("Attributes")]
    [SerializeField] private bool availableForAll;
    [SerializeField] private SOPlaceableObject[] availableFor;
    public Utils.SerializeableDict<GameManager.Resources, int> cost;
    public SOPerk perkRequired;
    public Mod[] modsToApply;

    public bool IsAvailable(PlaceableObject placeable_object) {
        if (
            !GameManager.instance.Game.CanSpendResources(cost.GetDict()) ||
            placeable_object.GetUpgrades().Contains(this) ||
            (perkRequired && !GameManager.instance.Game.perksUnlockTracker.unlocked[perkRequired.name])
        ) return false;
        return availableForAll || availableFor.Contains(placeable_object.placeableObjectSO);
    }
}
