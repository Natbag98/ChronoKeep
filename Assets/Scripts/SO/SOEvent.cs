using UnityEngine;

public abstract class SOEvent : ScriptableObject {
    [Header("SOEvent")]
    public string displayName;
    [SerializeField] protected string description;

    public virtual string GetDescription() { return description; }
    public virtual bool IsValid() { return true; }
    public virtual void Setup() {}
    public abstract void Event();
}
