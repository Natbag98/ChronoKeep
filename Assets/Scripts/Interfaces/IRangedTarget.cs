using UnityEngine;

public interface IRangedTarget {
    public Vector3 GetTargetPoint();
    public void Damage(float amount);
}
