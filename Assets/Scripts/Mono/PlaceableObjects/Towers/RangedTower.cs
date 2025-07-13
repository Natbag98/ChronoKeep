using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedTower : Tower {
    [Header("Ranged Tower")]
    [SerializeField] private GameObject projectileToShoot;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Utils.SerializableNullable<Transform> rotateTransform;
    [SerializeField] private float rotateSpeed;

    protected override void GetTarget() {
        List<Character> characters_in_range = new();
        foreach (Plot plot in GetPlotsInRange()) {
            foreach (Character character in plot.GetCharacters(false)) if (parentPlot.faction.atWarWith[character.faction]) characters_in_range.Add(character);
        }
        if (characters_in_range.Count > 0) target = Utils.Choice(characters_in_range).transform;
    }

    protected override IEnumerator Attack() {
        if (animator) animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.01f);
        if (animator) Debug.Log(attackFrame); // TODO : GPT this
        if (animator) yield return new WaitForSeconds(attackFrame / 60);
        Projectile projectile = Instantiate(
            projectileToShoot,
            shootPoint.position,
            Quaternion.identity,
            RunManager.instance.projectileContainer
        ).GetComponent<Projectile>();
        projectile.SetAttributes(attributes);
        projectile.SetTarget(target);
        projectile.SetMagicType(magicType);
        projectile.Setup();
    }

    protected override void Update() {
        if (target && rotateTransform.GetValue()) {
            Utils.RotateTowards(rotateTransform.GetValue().position, target.position, rotateTransform.GetValue(), rotateSpeed, "y");
        }
        if (target != null && target.GetComponent<IRangedTarget>().GetInvisible()) target = null;
        base.Update();
    }
}
