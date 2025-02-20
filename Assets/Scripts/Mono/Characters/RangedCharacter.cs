using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RangedCharacter : Character {
    [Header("Ranged Character")]
    [SerializeField] private GameObject projectileToShoot;
    [SerializeField] private Transform shootPoint;

    protected override void GetTarget() {
        List<PlaceableObject> towers_in_range = new();
        foreach (Plot plot in GetPlotsInRange()) {
            if (
                plot.GetComponentInChildren<PlaceableObject>() &&
                plot.faction != faction
            ) {
                towers_in_range.Add(plot.GetComponentInChildren<PlaceableObject>());
            }
        }
        if (towers_in_range.Count > 0) target = Utils.Choice(towers_in_range).transform;
    }

    private IEnumerator RangedAttack() {
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime);
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
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    protected override void Attack() { StartCoroutine(RangedAttack()); }

    protected override void Update() {
        base.Update();
        if (attacking) Utils.RotateTowards(transform.position, target.position, rotatePoint, rotateSpeed);
    }
}
