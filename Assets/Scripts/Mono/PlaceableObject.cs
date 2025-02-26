using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IRangedTarget {
    [Header("PlaceableObject")]
    [SerializeField] protected Attributes attributes;
    [SerializeField] private Transform centerPoint;

    [Header("PlaceableObject : References UI")]
    [SerializeField] private UnityEngine.UI.Image healthBar;

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

    protected virtual void UpdateUI() {
        healthBar.fillAmount = health / attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void Start() {
        health = attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void Update() {
        UpdateUI();

        if (health <= 0) {
            parentPlot.placedObjectType = null;
            Destroy(gameObject);
        }
    }
}
