using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class Character : MonoBehaviour, IRangedTarget {
    [Header("Attributes")]
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float attackDelayTime;
    [SerializeField] private GameManager.PlaceableObjectTypes[] movementTargetPriorities;
    [SerializeField] protected Attributes attributes;

    [Header("References")]
    [SerializeField] protected Transform centerPoint;
    [SerializeField] protected Transform rotatePoint;

    private float health;
    private Plot movementTarget;
    private List<Transform> path;
    private int pathIndex = 0;
    protected Transform target;
    protected bool canAttack = true;
    protected bool attacking;
    private Vector3 lastPosition;

    protected virtual void GetTarget() {}
    protected virtual void Attack() {}

    protected IEnumerator Reload() {
        canAttack = false;
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
            List<Plot> targets = RunManager.instance.GetAllPlotsWithPlacedObject(targetObjectType);
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
            movementTarget = RunManager.instance.GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Castle);
        } else {
            movementTarget = min_target;
        }
    }

    /// <summary>
    /// Gets and sets the character path.
    /// </summary>
    private void GetPath() {
        path = new();
        pathIndex = 0;
        foreach (Plot plot in Utils.GetPath(GetCurrentPlot().GetPositionInPlotArray(), movementTarget.GetPositionInPlotArray())) {
            path.Add(plot.transform);
        }
    }

    public Vector3 GetPathTargetPos() { return new Vector3(path[pathIndex].position.x, 0f, path[pathIndex].position.z); }

    private void Move() {
        Vector3 direction = (GetPathTargetPos() - transform.position).normalized;
        transform.Translate(attributes.GetAttribute(GameManager.Attributes.CharacterMoveSpeed) * Time.deltaTime * direction);
    }

    private void Rotate() {
        Utils.RotateTowards(transform.position, GetPathTargetPos(), rotatePoint, rotateSpeed);
    }

    private void CheckCollision() {
        // TODO : Set ray to cast from center point
        Vector3 direction = transform.position - lastPosition;
        Ray ray = new(lastPosition, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, lastPosition));
        foreach (RaycastHit hit in hits) {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform != null) {
                if (hit.transform.GetComponent<PlaceableObject>() != null) {
                    Debug.Log("Collided");
                }
            }
        }
        lastPosition = transform.position;
    }

    private void Start() {
        health = attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void UpdateAttack() {
        if (target == null) {
            GetTarget();
        } else {
            if (canAttack) {
                Attack();
                StartCoroutine(Reload());
            }
        }
    }

    protected virtual void Update() {
        if (movementTarget == null) {
            GetMovementTarget();
            GetPath();
        }

        if (!attacking) {
            Move();
            Rotate();
        }
        CheckCollision();
        UpdateAttack();

        if (Vector3.Distance(GetPathTargetPos(), transform.position) < 0.05f) {
            pathIndex++;
            if (pathIndex >= path.Count) {
                Destroy(gameObject);
            }
        }

        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
