using UnityEngine;

[CreateAssetMenu(fileName = "SOPlaceableObject", menuName = "SOPlaceableObject")]
public class SOPlaceableObject : ScriptableObject {
    [Header("References")]
    public GameObject placeableObjectPrefab;

    [Header("Attributes")]
    public GameManager.PlaceableObjectTypes objectType;
    public int factionControlRange;
}
