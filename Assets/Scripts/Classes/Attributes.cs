using UnityEngine;

[System.Serializable]
public class Attributes {
    [SerializeField] private Utils.SerializeableDict<GameManager.Attributes, float> baseAttributes;

    /// <summary>
    /// Gets the base value of the given attribute.
    /// </summary>
    /// <param name="attribute_to_get">The attribute to get.</param>
    /// <returns>The value of the attribute.</returns>
    public float GetAttribute(GameManager.Attributes attribute_to_get) {
        float attribute = baseAttributes.GetDict()[attribute_to_get];
        return attribute;
    }

    /// <summary>
    /// Gets the base value of the given attribute as an int.
    /// </summary>
    /// <param name="attribute_to_get">The attribute to get.</param>
    /// <returns>The value of the attribute.</returns>
    public int GetAttributeAsInt(GameManager.Attributes attribute_to_get) {
        float attribute = baseAttributes.GetDict()[attribute_to_get];
        return Mathf.FloorToInt(attribute);
    }
}
