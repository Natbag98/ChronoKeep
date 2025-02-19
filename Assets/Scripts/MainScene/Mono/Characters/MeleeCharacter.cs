using UnityEngine;

public class MeleeCharacter : Character {
    protected override void GetTarget() {
        if (blocked) {
            target = blockedObject.transform;
        }
    }

    protected override void Attack() {
        target.GetComponent<PlaceableObject>().Damage(attributes.GetAttribute(GameManager.Attributes.Attack));
    }

    protected override void Update() {
        if (blockedObject == null) blocked = false;
        base.Update();
    }
}
