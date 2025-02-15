using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour {
    [SerializeField] protected Attributes attributes;
    public GameManager.PlaceableObjectTypes objectType;
}
