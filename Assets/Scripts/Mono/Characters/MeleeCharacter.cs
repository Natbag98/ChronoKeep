using UnityEngine;
using System.Collections;

public class MeleeCharacter : Character {
    protected override void GetTarget() {
        if (blocked) {
            target = blockedObject.transform;
        }
    }

    private IEnumerator MeleeAttack() {
        attacking = true;
        canAttack = false;
        yield return new WaitForSeconds(attackDelayTime);
        target.GetComponent<PlaceableObject>().Damage(attributes.GetAttribute(GameManager.Attributes.Attack));
        attacking = false;

        StartCoroutine(Reload());
        reloadTimer = 0;
    }

    protected override void Attack() {
        StartCoroutine(MeleeAttack());
    }

    protected override void Update() {
        if (blockedObject == null) blocked = false;
        base.Update();
    }
}
