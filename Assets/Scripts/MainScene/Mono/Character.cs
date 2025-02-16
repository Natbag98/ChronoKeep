using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {
    [Header("Atttributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameManager.PlaceableObjectTypes[] targetPriorities;

    [Header("References")]
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform rotatePoint;

    private Plot movementTarget;
    private List<Transform> path;
    private int pathIndex = 0;

    private Plot GetCurrentPlot() {
        Ray ray = new(centerPos.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        foreach (RaycastHit hit in hits) if (hit.transform.TryGetComponent<Plot>(out Plot plot)) return plot;
        return null;
    }

    private void GetTarget() {
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
        transform.Translate(moveSpeed * Time.deltaTime * direction);
    }

    private void Rotate() {
        Utils.RotateTowards(transform.position, GetPathTargetPos(), rotatePoint, rotateSpeed);
    }

    private void Update() {
        if (movementTarget == null) {
            GetTarget();
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
    }
}
