using UnityEngine;

[System.Serializable]
public class Mod {
    public Tag.Tags[] targetTags;
    public GameManager.Attributes attributeToAffect;
    [Range(0, 2)] public float amount;

    /// <summary>
    /// Checks if the given set of tags contains the tags required for the mod.
    /// </summary>
    public bool CheckTags(Tag object_tags) {
        foreach (Tag.Tags tag in targetTags) {
            if (object_tags == null) return false;
            if (!object_tags.HasTag(tag)) return false;
        }

        return true;
    }
}
