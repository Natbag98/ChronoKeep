using System.Collections.Generic;
using UnityEngine;

public class RangedTower : Tower {
    [Header("Ranged Tower")]
    [SerializeField] private GameObject projectileToShoot;
    [SerializeField] private Transform shootPoint;

    protected override void GetTarget() {
        List<Character> characters_in_range = new();
        foreach (Plot plot in GetPlotsInRange()) characters_in_range.AddRange(plot.GetCharacters());
        if (characters_in_range.Count > 0) target = Utils.Choice(characters_in_range).transform;
    }

    protected override void Attack() {
        Projectile projectile = Instantiate(
            projectileToShoot,
            shootPoint.position,
            Quaternion.identity,
            Utils.GetManager<RunManager>().projectileContainer
        ).GetComponent<Projectile>();
        projectile.SetAttributes(attributes);
        projectile.SetTarget(target);
        projectile.Setup();
    }
}
