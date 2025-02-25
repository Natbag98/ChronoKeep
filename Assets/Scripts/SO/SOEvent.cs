using UnityEngine;

[CreateAssetMenu(fileName = "SOEvent", menuName = "SOEvent")]
public class SOEvent : ScriptableObject {
    [Header("SOEvent")]
    public string displayName;
    [SerializeField] private string description;

    public string GetDescription() { return description; }
    public bool IsValid() { return true; }
    public void Setup() {}
    public void Event() {}
}
