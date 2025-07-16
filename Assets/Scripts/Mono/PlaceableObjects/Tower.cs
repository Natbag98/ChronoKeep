using System.Collections;
using UnityEngine;

public abstract class Tower : PlaceableObject {
    [Header("Tower : References")]
    [SerializeField] protected SOSound shootSound;
    [SerializeField] private UnityEngine.UI.Image reloadBar;

    [Header("Animation")]
    [SerializeField] protected int attackFrame;

    protected Transform target;
    protected bool canAttack = true;
    protected float reloadTimer;

    protected virtual void GetTarget() {}
    protected virtual IEnumerator Attack() { yield return 0; }

    protected IEnumerator Reload() {
        canAttack = false;
        reloadTimer = 0;
        yield return new WaitForSeconds(attributes.GetAttribute(GameManager.Attributes.ReloadSpeed) / RunManager.instance.simSpeed);
        canAttack = true;
    }

    protected virtual void UpdateAttack() {
        if (target == null) {
            GetTarget();
        } else {
            if (canAttack) {
                shootSound.Play(gameObject);
                StartCoroutine(Attack());
                StartCoroutine(Reload());
            }
        }
    }

    protected override void UpdateUI() {
        base.UpdateUI();
        reloadBar.fillAmount = reloadTimer / attributes.GetAttribute(GameManager.Attributes.ReloadSpeed);
    }

    protected override void Start() {
        base.Start();
        reloadTimer = attributes.GetAttribute(GameManager.Attributes.ReloadSpeed);
    }

    protected override void Update() {
        reloadTimer += Time.deltaTime * RunManager.instance.simSpeed;
        base.Update();
        if (WaveManager.instance.waveActive) UpdateAttack();
    }
}
