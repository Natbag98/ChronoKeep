using UnityEngine;

public abstract class SOCharacterAddon : ScriptableObject {
    public virtual void AddonAwake(Character charater) {}
    public virtual void AddonStart(Character charater) {}
    public virtual void AddonUpdate(Character character) {}
}
