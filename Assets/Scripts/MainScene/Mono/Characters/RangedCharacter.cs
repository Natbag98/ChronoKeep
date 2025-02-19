using System.Collections.Generic;
using System.Collections;
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

    private IEnumerator RangedAttack() {
        attacking = true;
        yield return new WaitForSeconds(attackDelayTime / 2);
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
        yield return new WaitForSeconds(attackDelayTime / 2);
        attacking = false;
    }

    protected override void Attack() { StartCoroutine(RangedAttack()); }

    protected override void Update() {
        base.Update();
        if (attacking) Utils.RotateTowards(transform.position, target.position, rotatePoint, rotateSpeed);
    }
}
