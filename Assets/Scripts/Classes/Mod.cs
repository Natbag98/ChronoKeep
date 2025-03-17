using UnityEngine;

[System.Serializable]
public class Mod {
    public Tag.Tags[] targetTags;
    public GameManager.Attributes attributeToAffect;
    [Range(0, 2)] public float amount;
}
