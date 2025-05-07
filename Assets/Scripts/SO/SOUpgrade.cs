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

    public bool IsAvailable(SOPlaceableObject placeable_object) {
        if (!GameManager.instance.Game.CanSpendResources(cost.GetDict())) return false;
        return availableForAll || availableFor.Contains(placeable_object);
    }   
}
