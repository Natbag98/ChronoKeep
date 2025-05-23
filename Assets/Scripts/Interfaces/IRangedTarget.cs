using UnityEngine;

public interface IRangedTarget {
    public Vector3 GetTargetPoint();
    public void Damage(GameManager.MagicTypes attackType, float amount, Attributes attacker_attributes);
}
