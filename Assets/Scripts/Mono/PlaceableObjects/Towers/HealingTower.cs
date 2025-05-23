using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealingTower : Tower {
    [Header("Healing Tower")]
    [SerializeField] Tag.Tags[] targetTags;
    [SerializeField] int healCount;

    protected override void UpdateAttack() {
        if (canAttack) {
            Attack();
            StartCoroutine(Reload());
        }
    }

    protected override void Attack() {
        Dictionary<float, List<PlaceableObject>> potential_targets = new();
        foreach (Plot plot in parentPlot.GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range))) {
            PlaceableObject placeable_object = plot.GetComponentInChildren<PlaceableObject>();
            if (!placeable_object) continue;
            if (targetTags.Length != 0) if (!(bool)placeable_object.GetComponent<Tag>()?.HasTag(targetTags)) continue;
            if (potential_targets.ContainsKey(placeable_object.health)) {
                potential_targets[placeable_object.health].Add(placeable_object);
            } else {
                potential_targets.Add(placeable_object.health, new() { placeable_object });
            }
        }

        List<PlaceableObject> targets = new();
        if (healCount == 0) {
            foreach (float health in potential_targets.Keys) targets.AddRange(potential_targets[health]);
        } else {
            for (int i = 0; i < healCount; i++) {
                if (potential_targets.Count == 0) break;
                float target_health = potential_targets.Keys.Min();
                PlaceableObject target_to_add = Utils.Choice(potential_targets[target_health]);
                targets.Add(target_to_add);
                potential_targets[target_health].Remove(target_to_add);
                if (potential_targets[target_health].Count == 0) potential_targets.Remove(target_health);
            }
        }

        foreach (PlaceableObject target in targets) target.Heal(attributes.GetAttribute(GameManager.Attributes.Attack));
    }

    protected override void Start() {
        base.Start();
        WaveManager.instance.waveEnd += WaveEnd;
    }

    private void WaveEnd(object _, EventArgs __) {
        foreach (Plot plot in parentPlot.GetNeighbours()) {
            if (plot.GetComponentInChildren<PlaceableObject>()) plot.GetComponentInChildren<PlaceableObject>().Heal(0, true);
        }
    }
}
