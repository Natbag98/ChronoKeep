using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {
    [Header("Atttributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameManager.PlaceableObjectTypes[] targetPriorities;

    [Header("References")]
    [SerializeField] private Transform centerPos;

    private Plot movementTarget;

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

    private void Move() {
        Vector3 direction = (movementTarget.transform.position - transform.position).normalized;
        transform.Translate(moveSpeed * Time.deltaTime * direction);
    }

    private void Update() {
        if (movementTarget == null) {
            GetTarget();
            foreach (Plot t in Utils.GetPath(GetCurrentPlot().GetPositionInPlotArray(), movementTarget.GetPositionInPlotArray())) {
                Debug.Log($"{t.transform.position.x}, {t.transform.position.z}");
            }
        }
        Move();
    }
}
