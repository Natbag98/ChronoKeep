using UnityEngine;

public class CannonBall : Projectile {
    [SerializeField] private float arcHeightMultiplier;

    private Vector3 startPosition;
    private float stepScale;
    private float progress;
    private float arcHeight;
    private float explosionRadius;

    public override void SetAttributes(Attributes attributes) {
        base.SetAttributes(attributes);
        explosionRadius = attributes.GetAttribute(GameManager.Attributes.ExplosionRadius);
    }

    protected override void GetTargetPoint() { targetPoint = target.transform.position; }
    protected override void Collided(Collider coll) { Explode(); }

    public override void Setup() {
        base.Setup();
        startPosition = transform.position;
        stepScale = moveSpeed / Vector3.Distance(startPosition, targetPoint);
        arcHeight = arcHeightMultiplier * Vector3.Distance(startPosition, targetPoint);
    }

    protected override void Move() {
        progress = Mathf.Min(progress + Time.deltaTime * stepScale, 1.0f);
        float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);
        Vector3 nextPos = Vector3.Lerp(startPosition, targetPoint, progress);
        nextPos.y += parabola * arcHeight;
        transform.position = nextPos;

        if (progress == 1.0f) Explode();
    }

    private void Explode() {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits) hit.GetComponent<IRangedTarget>()?.Damage(magicType, damage);
        Destroy(gameObject);
    }
}
