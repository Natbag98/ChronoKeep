using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    protected float damage;
    protected float moveSpeed;
    protected Transform target;
    protected Vector3 targetPoint;

    public void SetTarget(Transform target) { this.target = target; }
    public void SetTargetPoint(Vector3 target_point) { targetPoint = target_point; }

    public virtual void SetAttributes(Attributes attributes) {
        damage = attributes.GetAttribute(GameManager.Attributes.Attack);
        moveSpeed = attributes.GetAttribute(GameManager.Attributes.ProjectileMoveSpeed);
    }

    private Vector3 lastPosition;

    protected abstract void Move();
    protected abstract void Collided(Collider coll);
    protected virtual void Hit(GameObject hit) { hit.GetComponent<IRangedTarget>().Damage(damage); Destroy(gameObject); }
    protected virtual void GetTargetPoint() { if (target != null) targetPoint = target.GetComponent<IRangedTarget>().GetTargetPoint(); }

    private void CheckCollision() {
        Vector3 direction = transform.position - lastPosition;
        Ray ray = new(lastPosition, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, lastPosition));
        foreach (RaycastHit hit in hits) {
            if (hit.transform != null) {
                if (hit.transform.GetComponent<IRangedTarget>() != null) {
                    Collided(hit.collider);
                }
            }
        }
        lastPosition = transform.position;
    }

    public virtual void Setup() { GetTargetPoint(); }
    private void Update() { Move(); CheckCollision(); }
    private void Start() { lastPosition = transform.position; }
}
