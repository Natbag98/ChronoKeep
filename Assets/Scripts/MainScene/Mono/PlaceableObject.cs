using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour {
    [Header("PlaceableObject")]
    [SerializeField] protected Attributes attributes;

    [HideInInspector] public GameManager.PlaceableObjectTypes objectType;
}
