using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attributes {
    [SerializeField] private Utils.SerializeableDict<GameManager.Attributes, float> baseAttributes;

    private List<Mod> mods = new();

    /// <summary>
    /// Gets the base value of the given attribute.
    /// </summary>
    /// <param name="attribute_to_get">The attribute to get.</param>
    /// <returns>The value of the attribute.</returns>
    public float GetAttribute(GameManager.Attributes attribute_to_get) {
        float attribute;
        if (HasAttribute(attribute_to_get)) {
            attribute = baseAttributes.GetDict()[attribute_to_get];
        } else {
            attribute = GameManager.instance.defaultAttributes.GetDict()[attribute_to_get];
        }

        foreach (Mod mod in mods) {
            if (mod.attributeToAffect == attribute_to_get) attribute *= mod.amount;
        }

        return attribute;
    }

    /// <summary>
    /// Gets the base value of the given attribute as an int.
    /// </summary>
    /// <param name="attribute_to_get">The attribute to get.</param>
    /// <returns>The value of the attribute.</returns>
    public int GetAttributeAsInt(GameManager.Attributes attribute_to_get) {
        return Mathf.FloorToInt(GetAttribute(attribute_to_get));
    }

    /// <summary>
    /// Attempts add a modifier to the attributes.
    /// </summary>
    /// <param name="mod">The mod to add.</param>
    /// <param name="object_tags">The tags required for the mod to be added.</param>
    /// <param name="allow_duplicate">Whether multiple instances of the same mod can be added.</param>
    /// <returns>Whether the mod was added to the attributes or not.</returns>
    public bool AddMod(Mod mod, Tag object_tags, bool allow_duplicate, Faction faction) {
        if (faction != null) {
            if (mod.applyTo == Mod.ApplyTo.Player) {
                if (faction != GameManager.instance.Game.PlayerFaction) return false;
            } else if (mod.applyTo == Mod.ApplyTo.Enemies) {
                if (faction == GameManager.instance.Game.PlayerFaction) return false;
            }   
        }

        if (
            !allow_duplicate && mods.Contains(mod) ||
            !mod.CheckTags(object_tags)
        ) return false;
        mods.Add(mod);
        return true;
    }

    public void RemoveMod(Mod mod) {
        mods.Remove(mod);
    }

    /// <summary>
    /// Checks whether the attributes class baseAttributes contains the given attribute
    /// </summary>
    public bool HasAttribute(GameManager.Attributes attribute) {
        return baseAttributes.GetDict().ContainsKey(attribute);
    }
}
