using UnityEngine;

[CreateAssetMenu(fileName = "SOCharacter", menuName = "SOCharacter")]
public class SOCharacter : ScriptableObject, IUnlockTrackable {
    [Header("References")]
    public GameObject prefab;

    [Header("Attributes")]
    public int powerRequired;

    [Header("UI")]
    public string displayName;
    public string description;

    public GameObject GetPrefab() { return prefab; }
}
