using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IRangedTarget, IMeleeTarget, IModable {
    [Header("PlaceableObject")]
    [SerializeField] protected Attributes attributes;
    [SerializeField] protected Utils.SerializeableDict<GameManager.Resources, int> resourcesPerWave;
    [SerializeField] private Transform centerPoint;

    [Header("PlaceableObject : References UI")]
    [SerializeField] private UnityEngine.UI.Image healthBar;

    [HideInInspector] public SOPlaceableObject placeableObjectSO;
    [HideInInspector] public GameManager.PlaceableObjectTypes objectType;
    [HideInInspector] public Plot parentPlot;
    [HideInInspector] public float health;

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(float amount) {
        if (Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense)) > 0) {
            health -= Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense));
        }
    }

    protected List<Plot> GetPlotsInRange() { return parentPlot.GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range)); }

    public void OnMouseEnter() { parentPlot.OnMouseEnter(); }
    public void OnMouseExit() { parentPlot.OnMouseExit(); }
    private void OnMouseDown() { parentPlot.OnMouseDown(); }

    private void WaveEnd(object _, EventArgs __) {
        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction) GameManager.instance.Game.AddResources(resourcesPerWave.GetDict());
    }

    protected virtual void UpdateUI() {
        healthBar.fillAmount = health / attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void Start() {
        health = attributes.GetAttribute(GameManager.Attributes.Health);
        WaveManager.instance.waveEnd += WaveEnd;
    }

    protected virtual void Update() {
        GameManager.instance.Game.placeableObjectsUnlockTracker.UpdateDiscovered(placeableObjectSO);
        UpdateUI();

        if (health <= 0) {
            parentPlot.placedObjectType = null;
            parentPlot.placedObjectSO = null;
            Destroy(gameObject);
        }
    }

    public void AddMod(Mod mod, bool allow_duplicate=false) {
        if (attributes.AddMod(mod, GetComponent<Tag>(), allow_duplicate) && mod.attributeToAffect == GameManager.Attributes.Health) {
            health *= mod.amount;
        }
    }

    public void RemoveMod(Mod mod) {
        attributes.RemoveMod(mod);
    }
}
