using System.Collections.Generic;
using UnityEngine;

public class RangedCharacter : Character {
    [Header("Ranged Character")]
    [SerializeField] private GameObject projectileToShoot;
    [SerializeField] private Transform shootPoint;

    protected override void GetTarget() {
        List<Tower> towers_in_range = new();
        foreach (Plot plot in GetPlotsInRange()) {
            if (plot.GetComponentInChildren<Tower>()) towers_in_range.Add(plot.GetComponentInChildren<Tower>());
        }
        if (towers_in_range.Count > 0) target = Utils.Choice(towers_in_range).transform;
    }

    protected override void Attack() {
        Projectile projectile = Instantiate(
            projectileToShoot,
            shootPoint.position,
            Quaternion.identity,
            RunManager.instance.projectileContainer
        ).GetComponent<Projectile>();
        projectile.SetDamage(attributes.GetAttribute(GameManager.Attributes.Attack));
        projectile.SetMoveSpeed(attributes.GetAttribute(GameManager.Attributes.ProjectileMoveSpeed));
        projectile.SetTarget(target);
        projectile.Setup();
    }
}
