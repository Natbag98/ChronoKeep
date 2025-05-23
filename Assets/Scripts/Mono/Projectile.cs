using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    protected Attributes attributes;
    protected Transform target;
    protected Vector3 targetPoint;
    protected GameManager.MagicTypes magicType;

    public void SetTarget(Transform target) { this.target = target; }
    public void SetTargetPoint(Vector3 target_point) { targetPoint = target_point; }
    public void SetMagicType(GameManager.MagicTypes magicType) { this.magicType = magicType; }
    public virtual void SetAttributes(Attributes attributes) { this.attributes = attributes; }

    private Vector3 lastPosition;

    protected abstract void Move();
    protected abstract void Collided(Collider coll);
    protected virtual void Hit(GameObject hit) {
        hit.GetComponent<IRangedTarget>().Damage(magicType, attributes.GetAttribute(GameManager.Attributes.Attack), attributes);
        Destroy(gameObject);
    }
    protected virtual void GetTargetPoint() { if (target != null) targetPoint = target.GetComponent<IRangedTarget>().GetTargetPoint(); }

    private void CheckCollision() {
        Vector3 direction = transform.position - lastPosition;
        Ray ray = new(lastPosition, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, lastPosition));
        foreach (RaycastHit hit in hits) {
            if (hit.transform == target) {
                if (hit.transform.GetComponent<IRangedTarget>() != null) {
                    Collided(hit.collider);
                }
            }
        }
        lastPosition = transform.position;
    }

    public virtual void Setup() { GetTargetPoint(); }
    private void Update() { if (RunManager.instance.paused) return; Move(); CheckCollision(); }
    private void Start() { lastPosition = transform.position; }
}
