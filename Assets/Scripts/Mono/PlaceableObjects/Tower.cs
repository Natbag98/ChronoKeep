using System.Collections;
using UnityEngine;

public abstract class Tower : PlaceableObject {
    [Header("Tower : References")]
    [SerializeField] private UnityEngine.UI.Image reloadBar;

    protected Transform target;
    protected bool canAttack = true;
    protected float reloadTimer;

    protected virtual void GetTarget() {}
    protected virtual void Attack() {}

    protected IEnumerator Reload() {
        canAttack = false;
        reloadTimer = 0;
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

    protected override void UpdateUI() {
        base.UpdateUI();
        reloadBar.fillAmount = reloadTimer / attributes.GetAttribute(GameManager.Attributes.ReloadTime);
    }

    protected override void Start() {
        base.Start();
        reloadTimer = attributes.GetAttribute(GameManager.Attributes.ReloadTime);
    }

    protected override void Update() {
        reloadTimer += Time.deltaTime;
        base.Update();
        if (Utils.GetManager<WaveManager>().waveActive) UpdateAttack();
    }
}
