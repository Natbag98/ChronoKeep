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
                targets_in_range.Add(plot.GetComponentInChildren<PlaceableObject>().transform);
            }

            foreach (Character character in plot.GetCharacters(false)) {
                if (faction.atWarWith[character.faction]) targets_in_range.Add(character.transform);
            }
        }

        if (targets_in_range.Count > 0) target = Utils.Choice(targets_in_range);
    }

    private IEnumerator RangedAttack() {
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime / RunManager.instance.simSpeed);
        if (!attacking) yield break;
        Projectile projectile = Instantiate(
            projectileToShoot,
            shootPoint.position,
            Quaternion.identity,
            RunManager.instance.projectileContainer
        ).GetComponent<Projectile>();
        projectile.SetAttributes(attributes);
        projectile.SetTarget(target);
        projectile.SetMagicType(magicType);
        projectile.Setup();
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    private IEnumerator RangedMeleeAttack() {
        Debug.Log("here");
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime / RunManager.instance.simSpeed);
        if (!attacking) yield break;
        target.GetComponent<IMeleeTarget>().Damage(
            magicType,
            attributes.GetAttribute(GameManager.Attributes.Attack) * (1 - attributes.GetAttribute(GameManager.Attributes.RangedMeleeAttackReduction) / 100), 
            attributes
        );
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    private void CheckTargetInRange() {
        Character target_character = target.GetComponentInParent<Character>();
        if (target_character != null) {
            if (!GetPlotsInRange().Contains(target_character.GetCurrentPlot())) target = null;
        } else {
            if (!GetPlotsInRange().Contains(target.gameObject.GetComponent<PlaceableObject>().GetComponentInParent<Plot>())) target = null;
        }
    }

    protected override void Attack() {
        if (!blocked) {
            StartCoroutine(RangedAttack());
        } else {
            StartCoroutine(RangedMeleeAttack());
        }
    }

    protected override void UpdateAttack() {
        base.UpdateAttack();
        if (blocked && target == null) {
            Attack();
        }
    }

    protected override void Update() {
        base.Update();
        if (target != null) CheckTargetInRange();
        if (target == null) attacking = false;
        if (attacking) Utils.RotateTowards(transform.position, target.position, rotatePoint, rotateSpeed);
    }
}
