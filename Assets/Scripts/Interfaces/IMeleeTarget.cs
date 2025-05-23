using UnityEngine;

public interface IMeleeTarget {
    public void Damage(GameManager.MagicTypes attackType, float amount, Attributes attacker_attributes);
}
