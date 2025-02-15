using UnityEngine;

[CreateAssetMenu(fileName = "SOPlaceableObject", menuName = "SOPlaceableObject")]
public class SOPlaceableObject : ScriptableObject {
    public GameObject placeableObjectPrefab;
    public GameManager.PlaceableObjectTypes objectType;
}
