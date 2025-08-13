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

        float targetTime = 0;
        if (animator) {
            float timePerFrame = 1f / animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;
            targetTime = attackFrame * timePerFrame * RunManager.instance.simSpeed;
        }
        if (animator) yield return new WaitForSeconds(targetTime);

        if (GetComponent<Tag>().HasTag(Tag.Tags.OilTower)) {
            foreach (Plot neighbour in GetComponentInParent<Plot>().GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range))) {
                foreach (Character character in neighbour.GetCharacters()) {
                    if (character.faction != GameManager.instance.Game.PlayerFaction) {
                        character.AddMod(
                            new() {
                                targetTags = new Tag.Tags[0],
                                attributeToAffect = GameManager.Attributes.CharacterMoveSpeed,
                                amount = 0.6f,
                                applyTo = Mod.ApplyTo.All
                            },
                            true
                        );
                        character.AddMod(
                            new() {
                                targetTags = new Tag.Tags[0],
                                attributeToAffect = GameManager.Attributes.ReloadSpeed,
                                amount = 0.8f,
                                applyTo = Mod.ApplyTo.All
                            },
                            true
                        );
                        character.oil = true;
                    }
                }
            }
            yield break;
        }

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

    private void CheckTargetInRange() {
        if (!GetPlotsInRange().Contains(target.GetComponent<Character>().GetCurrentPlot())) target = null;
    }

    protected override void Update() {
        if (target != null) CheckTargetInRange();
        if (target && rotateTransform.GetValue()) {
            Utils.RotateTowards(rotateTransform.GetValue().position, target.position, rotateTransform.GetValue(), rotateSpeed, "y");
        }
        if (target != null && target.GetComponent<IRangedTarget>().GetInvisible()) target = null;
        base.Update();
    }
}
