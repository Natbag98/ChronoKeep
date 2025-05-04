using System;
using System.Collections.Generic;
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

    public int GetRange() {
        return attributes.GetAttributeAsInt(GameManager.Attributes.Range);
    }

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(GameManager.MagicTypes attackType, float amount) {
        if (Utils.CalculateDamage(attackType, amount, attributes) > 0) {
            health -= Utils.CalculateDamage(attackType, amount, attributes);
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
