using UnityEngine;

[CreateAssetMenu(fileName = "SOPlaceableObject", menuName = "SOPlaceableObject")]
public class SOPlaceableObject : ScriptableObject {
    [Header("References")]
    public GameObject placeableObjectPrefab;

    [Header("Attributes")]
    public string displayName;
    public GameManager.PlaceableObjectTypes objectType;
    public int factionControlRange;
    public Utils.SerializeableDict<GameManager.Resources, int> placementCost;

    [Header("Placement Rules")]
    public GameManager.PlotTypes[] mustPlaceBeside;
}
