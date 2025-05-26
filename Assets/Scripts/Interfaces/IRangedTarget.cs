using UnityEngine;

public interface IRangedTarget {
    public bool GetInvisible();
    public Vector3 GetTargetPoint();
    public void Damage(GameManager.MagicTypes attackType, float amount, Attributes attacker_attributes);
}
