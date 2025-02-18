using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IRangedTarget {
    [Header("PlaceableObject")]
    [SerializeField] protected Attributes attributes;
    [SerializeField] private Transform centerPoint;

    [HideInInspector] public GameManager.PlaceableObjectTypes objectType;
    [HideInInspector] public Plot parentPlot;
    private float health;

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(float amount) {
        if (Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense)) > 0) {
            health -= Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense));
        }
    }

    protected List<Plot> GetPlotsInRange() { return parentPlot.GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range)); }

    private void Start() {
        health = attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void Update() {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
