using UnityEngine;

[System.Serializable]
public class Attributes {
    [SerializeField] private Utils.SerializeableDict<GameManager.Attributes, float> baseAttributes;

    public float GetAttribute(GameManager.Attributes attribute_to_get) {
        float attribute = baseAttributes.GetDict()[attribute_to_get];
        return attribute;
    }
}
