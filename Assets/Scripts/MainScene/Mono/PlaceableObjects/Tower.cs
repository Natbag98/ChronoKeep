using System.Collections;
using UnityEngine;

public abstract class Tower : PlaceableObject {
    protected Transform target;
    protected bool canAttack = true;

    protected virtual void GetTarget() {}
    protected virtual void Attack() {}

    protected IEnumerator Reload() {
        canAttack = false;
        yield return new WaitForSeconds(attributes.GetAttribute(GameManager.Attributes.ReloadTime));
        canAttack = true;
    }

    protected virtual void UpdateAttack() {
        if (target == null) {
            GetTarget();
        } else {
            if (canAttack) {
                Attack();
                StartCoroutine(Reload());
            }
        }
    }

    protected override void Update() {
        base.Update();
        UpdateAttack();
    }
}
