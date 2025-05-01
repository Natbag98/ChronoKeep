using UnityEngine;

[CreateAssetMenu(fileName = "SOFeature", menuName = "SOFeature")]
public class SOFeature : ScriptableObject {
    [Header("References")]
    public GameObject prefab;

    [Header("UI")]
    public string displayName;
    public string description;
}
