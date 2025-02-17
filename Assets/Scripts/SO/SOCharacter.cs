using UnityEngine;

[CreateAssetMenu(fileName = "SOCharacter", menuName = "SOCharacter")]
public class SOCharacter : ScriptableObject {
    [Header("References")]
    public GameObject prefab;

    [Header("Attributes")]
    public int powerRequired;    
}
