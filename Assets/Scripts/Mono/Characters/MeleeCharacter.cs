using UnityEngine;
using System.Collections;
using System.Linq;

public class MeleeCharacter : Character {
    protected override void GetTarget() {
        if (blocked) {
            if (blockedObject == null) {
                target = Utils.Choice(
                    (from character in GetCurrentPlot().GetCharacters() where faction.atWarWith[character.faction] select character).ToList()
                ).transform;
            } else {
                target = blockedObject.transform;
            }
        }
    }

    private IEnumerator MeleeAttack() {
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime / RunManager.instance.simSpeed);
        if (!attacking) yield break;
        Debug.Log(attributes.GetAttribute(GameManager.Attributes.DamageReductionTower));
        target.GetComponent<IMeleeTarget>().Damage(magicType, attributes.GetAttribute(GameManager.Attributes.Attack), attributes);
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    protected override void Attack() {
        StartCoroutine(MeleeAttack());
    }

    protected override void Update() {
        base.Update();
        if (target == null) attacking = false;
    }
}
