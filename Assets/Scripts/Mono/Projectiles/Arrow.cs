using UnityEngine;

public class Arrow : Projectile {
    public override void Setup() { base.Setup(); Utils.RotateTowards(transform.position, targetPoint, transform); }
    protected override void Collided(Collider coll) { Hit(coll.gameObject); }
    protected override void Move() { transform.Translate(attributes.GetAttribute(GameManager.Attributes.ProjectileMoveSpeed) * Time.deltaTime * Vector3.forward * RunManager.instance.simSpeed); }
}
