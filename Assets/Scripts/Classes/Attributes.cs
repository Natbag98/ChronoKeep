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
        float attribute = baseAttributes.GetDict()[attribute_to_get];
        foreach (Mod mod in mods) {
            Debug.Log(mod);
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
        float attribute = baseAttributes.GetDict()[attribute_to_get];
        foreach (Mod mod in mods) {
            Debug.Log(mod);
            if (mod.attributeToAffect == attribute_to_get) attribute *= mod.amount;
        }
        return Mathf.FloorToInt(attribute);
    }

    public void AddMod(Mod mod) {
        mods.Add(mod);
    }
}
