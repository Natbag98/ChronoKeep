using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IRangedTarget, IMeleeTarget, IModable {
    [Header("PlaceableObject")]
    [SerializeField] protected GameManager.MagicTypes magicType;
    [SerializeField] protected Attributes attributes;
    [SerializeField] protected Utils.SerializeableDict<GameManager.Resources, int> resourcesPerWave;
    [SerializeField] private Transform centerPoint;

    [Header("PlaceableObject : References UI")]
    [SerializeField] private UnityEngine.UI.Image healthBar;

    [HideInInspector] public SOPlaceableObject placeableObjectSO;
    [HideInInspector] public GameManager.PlaceableObjectTypes objectType;
    [HideInInspector] public Plot parentPlot;
    [HideInInspector] public float health;

    [HideInInspector] public bool loaded = false;
    private float loadedTimer = 2;

    public int GetRange(Plot hoverPlot) {
        if (!attributes.HasAttribute(GameManager.Attributes.Range)) return 0;
        float range = attributes.GetAttributeAsInt(GameManager.Attributes.Range);
        foreach (Mod mod in hoverPlot.modsToApply) {
            if (mod.attributeToAffect == GameManager.Attributes.Range && mod.CheckTags(GetComponent<Tag>())) range *= mod.amount;
        }
        return (int)Math.Floor(range);
    }

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(GameManager.MagicTypes attackType, float amount, Attributes attacker_attributes) {
        if (Utils.CalculateDamage(attackType, amount, attributes, attacker_attributes.GetAttribute(GameManager.Attributes.DamageReductionTower)) > 0) {
            health -= Utils.CalculateDamage(attackType, amount, attributes, attacker_attributes.GetAttribute(GameManager.Attributes.DamageReductionTower));
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

    private void SetVisible(bool set) {
        if (GameManager.instance.debugMode) return;
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = set;
        foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) canvas.enabled = set;
    }

    protected virtual void Start() {
        if (!parentPlot.visibleToPlayer) SetVisible(false);
        if (!loaded) health = attributes.GetAttribute(GameManager.Attributes.Health);
        WaveManager.instance.waveEnd += WaveEnd;

        if (parentPlot.faction == GameManager.instance.Game.PlayerFaction && attributes.HasAttribute(GameManager.Attributes.Range)) {
            foreach (Plot plot in parentPlot.GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range))) {
                plot.SetVisibleToPlayer(true);
            }
        }

        foreach (Mod mod in parentPlot.modsToApply) {
            attributes.AddMod(mod, GetComponent<Tag>(), false);
        }
    }

    protected virtual void Update() {
        loadedTimer -= Time.deltaTime;
        GameManager.instance.Game.placeableObjectsUnlockTracker.UpdateDiscovered(placeableObjectSO);
        UpdateUI();

        if (health <= 0) {
            DestroySelf();
        }
    }

    protected virtual void DestroySelf() {
        parentPlot.placedObjectSO = null;
        parentPlot.PlaceFeature(GameManager.instance.Ruins);
        Destroy(gameObject);
    }

    public void AddMod(Mod mod, bool allow_duplicate=false) {
        if (attributes.AddMod(mod, GetComponent<Tag>(), allow_duplicate) && mod.attributeToAffect == GameManager.Attributes.Health) {
            if (loaded && loadedTimer > 0) return;
            health *= mod.amount;
        }
    }

    public void RemoveMod(Mod mod) {
        attributes.RemoveMod(mod);
    }
}
