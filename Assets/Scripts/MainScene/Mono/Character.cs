using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour, IRangedTarget {
    [Header("Attributes")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameManager.PlaceableObjectTypes[] targetPriorities;
    [SerializeField] protected Attributes attributes;

    [Header("References")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform rotatePoint;

    private float health;
    private Plot movementTarget;
    private List<Transform> path;
    private int pathIndex = 0;

    public Vector3 GetTargetPoint() { return centerPoint.position; }
    public void Damage(float amount) {
        if (Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense)) > 0) {
            health -= Utils.CalculateDamage(amount, attributes.GetAttribute(GameManager.Attributes.Defense));
        }
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
        foreach (GameManager.PlaceableObjectTypes targetObjectType in targetPriorities) {
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

    private void Start() {
        health = attributes.GetAttribute(GameManager.Attributes.Health);
    }

    protected virtual void Update() {
        if (movementTarget == null) {
            GetMovementTarget();
            GetPath();
        }

        Move();
        Rotate();

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
