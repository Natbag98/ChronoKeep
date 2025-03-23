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

    protected virtual void GetTarget() {}
    protected virtual void Attack() {}

    protected IEnumerator Reload() {
        yield return new WaitForSeconds(attributes.GetAttribute(GameManager.Attributes.ReloadTime));
        canAttack = true;
    }

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(float amount) {
        if (Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense)) > 0) {
            health -= Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense));
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
    /// Gets and sets the chracters movement target.
    /// </summary>
    private void GetMovementTarget() {
        Plot min_target = null;
        float? min_distance = null;
        foreach (GameManager.PlaceableObjectTypes targetObjectType in movementTargetPriorities) {
            List<Plot> targets = Utils.GetManager<RunManager>().GetAllPlotsWithPlacedObject(targetObjectType, targetFaction);
            foreach (Plot target in targets) {
                float distance = Vector2.Distance(target.transform.position, transform.position);
                min_distance ??= distance; min_target = min_target != null ? min_target : target;
                if (distance < min_distance) {
                    min_distance = distance;
                    min_target = target;
                }
            }
        }

        if (min_target == null) {
            movementTarget = Utils.GetManager<RunManager>().GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Castle, targetFaction);
            if (movementTarget == null) movementTarget = Utils.Choice(Utils.GetManager<RunManager>().GetAllPlotsWithFactionObjects(targetFaction));
        } else {
            movementTarget = min_target;
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

    private void Start() {
        List<Plot> targets = Utils.GetManager<RunManager>().GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Castle);
        Dictionary<float, Plot> potential_targets = new();
        foreach (Plot target in targets) {
            if (faction.atWarWith[target.faction]) potential_targets.Add(Vector3.Distance(target.transform.position, transform.position), target);
        }
        targetFaction = potential_targets[potential_targets.Keys.ToArray().Min()].faction;

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
