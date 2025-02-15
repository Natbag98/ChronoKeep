using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {
    [Header("Atttributes")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameManager.PlaceableObjectTypes[] targetPriorities;

    private Plot movementTarget;

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
        if (movementTarget == null) GetTarget();
        Move();
    }
}
