using UnityEngine;

[CreateAssetMenu(fileName = "ApplyMods", menuName = "Event/ApplyMods")]
public class ApplyMods : SOEvent {
    [Header("Attributes")]
    [SerializeField] Mod[] modsToApply;

    public override string GetDescription() {
        string description = this.description;
        description += "\n\nMods Applied:\n";
        foreach (Mod mod in modsToApply) {
            string target_tags = "";
            foreach (Tag.Tags tag in mod.targetTags) {
                target_tags += $" {tag.ToString().ToLower()}";
            }

            string apply = "";
            if (mod.applyTo != Mod.ApplyTo.All) {
                apply = $" {mod.applyTo.ToString().ToLower()} ";
            }

            if (mod.amount >= 1) {
                description += $"All{apply}{target_tags}'s will gain {(mod.amount - 1) * 100}% {mod.attributeToAffect.ToString().ToLower()}\n";
            } else {
                description += $"All{apply}{target_tags}'s will lose {(1 - mod.amount) * 100}% {mod.attributeToAffect.ToString().ToLower()}\n";
            }
        }
        return description;
    }

    public override void Event() {
        foreach (Mod mod in modsToApply) {
            RunManager.instance.globalMods.Add(
                new() {
                    targetTags = mod.targetTags,
                    attributeToAffect = mod.attributeToAffect,
                    amount = mod.amount,
                    applyTo = mod.applyTo
                }
            );
        }
    }
}
