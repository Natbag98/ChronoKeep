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
                target_tags += $"{tag} ";
            }

            if (mod.amount >= 1) {
                description += $"All {target_tags}will gain {(mod.amount - 1) * 100}% {mod.attributeToAffect}\n";
            } else {
                description += $"All {target_tags}will lose {(1 - mod.amount) * 100}% {mod.attributeToAffect}\n";
            }
        }
        return description;
    }

    public override void Event() {
        RunManager.instance.globalMods.AddRange(modsToApply);
    }
}
