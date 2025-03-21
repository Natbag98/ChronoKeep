using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RangedCharacter : Character {
    [Header("Ranged Character")]
    [SerializeField] private GameObject projectileToShoot;
    [SerializeField] private Transform shootPoint;

    protected override void GetTarget() {
        List<Transform> targets_in_range = new();

        foreach (Plot plot in GetPlotsInRange()) {
            if (
                plot.GetComponentInChildren<PlaceableObject>() &&
                faction.atWarWith[plot.faction]
            ) {
                targets_in_range.Add(plot.transform);
            }

            foreach (Character character in plot.GetCharacters()) {
                if (faction.atWarWith[character.faction]) targets_in_range.Add(character.transform);
            }
        }

        if (targets_in_range.Count > 0) target = Utils.Choice(targets_in_range);
    }

    private IEnumerator RangedAttack() {
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime);
        if (!attacking) yield break;
        Projectile projectile = Instantiate(
            projectileToShoot,
            shootPoint.position,
            Quaternion.identity,
            Utils.GetManager<RunManager>().projectileContainer
        ).GetComponent<Projectile>();
        projectile.SetAttributes(attributes);
        projectile.SetTarget(target);
        projectile.Setup();
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    protected override void Attack() { StartCoroutine(RangedAttack()); }

    protected override void Update() {
        base.Update();
        if (target == null) attacking = false;
        if (attacking) Utils.RotateTowards(transform.position, target.position, rotatePoint, rotateSpeed);
    }
}
