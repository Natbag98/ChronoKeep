using UnityEngine;

public class Arrow : Projectile {
    [SerializeField] private int moveSpeed;

    public override void Setup() { base.Setup(); Utils.RotateTowards(transform.position, targetPoint, transform); }
    protected override void Collided(Collider coll) { Hit(coll.gameObject); }
    protected override void Move() { transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward); }
}
