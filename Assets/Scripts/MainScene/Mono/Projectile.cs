using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    protected float damage;
    protected Transform target;
    protected Vector3 targetPoint;

    public void SetDamage(float damage) { this.damage = damage; }
    public void SetTarget(Transform target) { this.target = target; }
    public void SetTargetPoint(Vector3 target_point) { targetPoint = target_point; }

    private Vector3 lastPosition;

    protected abstract void Move();
    protected abstract void Collided(Collider coll);
    protected virtual void Hit(GameObject hit) { Debug.Log(hit.GetComponent<IRangedTarget>()); Destroy(gameObject); }
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
