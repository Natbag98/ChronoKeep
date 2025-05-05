using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Mathematics;
using System.Linq;

public abstract class Character : MonoBehaviour, IRangedTarget, IMeleeTarget, IModable {
    [Header("Attributes")]
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float attackDelayTime;
    [SerializeField] protected GameManager.MagicTypes magicType;
    [SerializeField] private GameManager.PlaceableObjectTypes[] movementTargetPriorities;
    [SerializeField] protected Attributes attributes;

    [Header("References")]
    [SerializeField] protected Transform centerPoint;
    [SerializeField] protected Transform rotatePoint;

    [Header("References UI")]
    [SerializeField] private UnityEngine.UI.Image healthBar;
    [SerializeField] private UnityEngine.UI.Image reloadBar;

    private Vector2 moveOffset;
    [HideInInspector] public Faction faction;
    [HideInInspector] public SOCharacter characterSO;
    private float health;
    private Faction targetFaction;
    private Plot movementTarget;
    private List<Transform> path;
    private int pathIndex = 0;
    protected Transform target;
    protected bool canAttack = true;
    protected bool attacking;
    private Vector3 lastPosition;
    protected bool blocked = false;
    protected PlaceableObject blockedObject;
    protected float reloadTimer;
    private Plot fromPlot;
    private List<Mod> modsFromPlot = new();

    protected virtual void GetTarget() {}
    protected virtual void Attack() {}

    protected IEnumerator Reload() {
        yield return new WaitForSeconds(attributes.GetAttribute(GameManager.Attributes.ReloadTime));
        canAttack = true;
    }

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(GameManager.MagicTypes attackType, float amount) {
        if (Utils.CalculateDamage(attackType, amount, attributes) > 0) {
            health -= Utils.CalculateDamage(attackType, amount, attributes);
        }
    }

    protected List<Plot> GetPlotsInRange() { return GetCurrentPlot().GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range)); }

    protected virtual void UpdateUI() {
        healthBar.fillAmount = health / attributes.GetAttribute(GameManager.Attributes.Health);
        reloadBar.fillAmount = reloadTimer / (attributes.GetAttribute(GameManager.Attributes.ReloadTime) + attackDelayTime);
    }

    /// <summary>
    /// Gets the plot that the character is currently standing on.
    /// </summary>
    /// <returns>The plot the character is currently standing on.</returns>
    public Plot GetCurrentPlot() {
        Ray ray = new(centerPoint.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        foreach (RaycastHit hit in hits) if (hit.transform.TryGetComponent(out Plot plot)) return plot;
        return null;
    }

    /// <summary>
    /// Gets and sets the characters movement target.
    /// </summary>
    private void GetMovementTarget() {
        Plot min_target = null;
        float? min_distance = null;
        List<Plot> target_objects = (
            from plot
            in RunManager.instance.GetAllPlotsWithFactionObjects(targetFaction)
            where plot.walkable
            select plot
        ).ToList();

        // Target closest priority target
        foreach (GameManager.PlaceableObjectTypes targetObjectType in movementTargetPriorities) {
            List<Plot> targets = RunManager.instance.GetAllPlotsWithPlacedObject(targetObjectType, targetFaction);
            foreach (Plot target in targets) {
                float distance = Vector2.Distance(target.transform.position, transform.position);
                min_distance ??= distance; min_target = min_target != null ? min_target : target;
                if (distance < min_distance) {
                    min_distance = distance;
                    min_target = target;
                }
            }
        }

        // Attempt to target castle if there are no priority targets
        if (min_target == null) {
            movementTarget = RunManager.instance.GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Castle, targetFaction);
            if (movementTarget == null) movementTarget = Utils.Choice(target_objects);
        } else {
            movementTarget = min_target;
        }

        // Attack the closest enemy object if the castle and priority targets have no valid paths
        if (Utils.GetPath(GetCurrentPlot().GetPositionInPlotArray(), movementTarget.GetPositionInPlotArray()) == null) {
            Dictionary<float, Plot> potential_movement_targets = new();
            foreach (Plot plot in target_objects) {
                if (Utils.GetPath(GetCurrentPlot().GetPositionInPlotArray(), plot.GetPositionInPlotArray()) != null) {
                    potential_movement_targets.Add(Vector3.Distance(transform.position, plot.transform.position), plot);
                }
            }
            movementTarget = potential_movement_targets[potential_movement_targets.Keys.ToArray().Min()];
        }
    }

    /// <summary>
    /// Gets and sets the character path.
    /// </summary>
    private void GetPath() {
        GetMovementTarget();

        path = new();
        pathIndex = 0;
        List<Plot> new_path = Utils.GetPath(GetCurrentPlot().GetPositionInPlotArray(), movementTarget.GetPositionInPlotArray());

        if (new_path == null) {
            // Later in development enemies should either not spawn from a spawner with no valid path or the enemy should attempt to attack a different faction
            Debug.Log("No path found");
            Destroy(gameObject);
        }

        foreach (Plot plot in new_path) {
            path.Add(plot.transform);
        }
    }

    public Vector3 GetPathTargetPos() {
        return new Vector3(
            path[pathIndex].position.x - moveOffset.x,
            0f,
            path[pathIndex].position.z - moveOffset.y
        );
    }

    private void Move() {
        Vector3 direction = (GetPathTargetPos() - transform.position).normalized;
        transform.Translate(attributes.GetAttribute(GameManager.Attributes.CharacterMoveSpeed) * Time.deltaTime * direction);
    }

    private void Rotate() {
        Utils.RotateTowards(transform.position, GetPathTargetPos(), rotatePoint, rotateSpeed);
    }

    private void CheckCollision() {
        Vector3 temp_current_pos = new(transform.position.x, centerPoint.position.y, transform.position.z);
        Vector3 temp_last_pos  = new(lastPosition.x, centerPoint.position.y, lastPosition.z);
        Vector3 direction = temp_current_pos - temp_last_pos;
        Ray ray = new(temp_last_pos, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(temp_current_pos, temp_last_pos));
        foreach (RaycastHit hit in hits) {
            if (hit.transform != null) {
                if (
                    hit.transform.GetComponent<PlaceableObject>() != null &&
                    hit.transform.GetComponent<PlaceableObject>().parentPlot.faction != faction
                ) {
                    blocked = true;
                    blockedObject = hit.transform.GetComponent<PlaceableObject>();
                }
            }
        }
        lastPosition = transform.position;
    }

    private void CheckCollisionWithCharacters() {
        foreach (Character character in GetCurrentPlot().GetCharacters()) {
            if (faction.atWarWith[character.faction]) {
                Debug.Log("Coll");
                blocked = true;
            }
        }
    }

    private void SetVisible(bool set) {
        if (GameManager.instance.debugMode) return;
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = set;
        foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) canvas.enabled = set;
    }

    public void SetStartPos(Vector3 position) { 
        lastPosition = position;
        transform.position = new Vector3(
            position.x - moveOffset.x,
            0f,
            position.z - moveOffset.y
        );
    }

    private void Awake() {
        moveOffset = new(
            Mathf.Max(Mathf.Min((float)GameManager.Random.NextDouble() - 0.5f, 0.4f), -0.4f),
            Mathf.Max(Mathf.Min((float)GameManager.Random.NextDouble() - 0.5f, 0.4f), -0.4f)
        );
    }

    private void GetTargetFaction() {
        List<Plot> targets = (
            from plot
            in RunManager.instance.GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Castle)
            where plot.visibleToPlayer
            select plot
        ).ToList();
        Dictionary<float, Plot> potential_targets = new();

        foreach (Plot target in targets) {
            if (faction.atWarWith[target.faction]) potential_targets.Add(Vector3.Distance(target.transform.position, transform.position), target);
        }

        if (
            faction == GameManager.instance.Game.PlayerFaction &&
            RunManager.instance.GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Spawner, GameManager.instance.Game.BaseFactions[^1]) != null
        ) {
            potential_targets.Add(
                0,
                RunManager.instance.GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Spawner, GameManager.instance.Game.BaseFactions[^1])
            );
        }
    
        targetFaction = potential_targets[potential_targets.Keys.ToArray().Min()].faction;
    }

    private void Start() {
        SetVisible(false);
        GetTargetFaction();

        health = attributes.GetAttribute(GameManager.Attributes.Health);
        reloadTimer = attributes.GetAttribute(GameManager.Attributes.ReloadTime);
    }

    protected virtual void UpdateAttack() {
        if (target == null) {
            GetTarget();
        } else {
            if (canAttack) {
                Attack();
            }
        }
    }

    protected virtual void Update() {
        reloadTimer += Time.deltaTime;
        if (movementTarget == null) GetPath();
        if (blockedObject == null) blocked = false;
        if (health <= 0) Destroy(gameObject);
        SetVisible(GetCurrentPlot().visibleToPlayer);

        CheckCollision();
        CheckCollisionWithCharacters();
        UpdateAttack();
        UpdateUI();

        if (attacking) {
            if (reloadTimer > attributes.GetAttribute(GameManager.Attributes.ReloadTime) + attackDelayTime) {
                reloadTimer = attributes.GetAttribute(GameManager.Attributes.ReloadTime) + attackDelayTime;
            }
        } else {
            if (reloadTimer > attributes.GetAttribute(GameManager.Attributes.ReloadTime)) {
                reloadTimer = attributes.GetAttribute(GameManager.Attributes.ReloadTime);
            }
            if (!blocked) Move();
            Rotate();
        }

        if (Vector3.Distance(GetPathTargetPos(), transform.position) < 0.05f) {
            pathIndex++;
            if (pathIndex >= path.Count) {
                Destroy(gameObject);
            }
        }

        if (faction == GameManager.instance.Game.PlayerFaction) {
            Plot plot = GetCurrentPlot();
            plot.SetVisibleToPlayer(true);
            foreach (Plot n_plot in plot.GetNeighbours(square: true)) n_plot.SetVisibleToPlayer(true);
        }

        if (GetCurrentPlot().visibleToPlayer) {
            GameManager.instance.Game.characterUnlockTracker.UpdateDiscovered(characterSO);
        }

        if (fromPlot != GetCurrentPlot()) {
            fromPlot = GetCurrentPlot();
            foreach (Mod mod in modsFromPlot) attributes.RemoveMod(mod);
            modsFromPlot.Clear();
            foreach (Mod mod in GetCurrentPlot().modsToApply) {
                attributes.AddMod(mod, GetComponent<Tag>(), false);
                modsFromPlot.Add(mod);
            }
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
