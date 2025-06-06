using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWar", menuName = "Event/RandomWar")]
public class RandomWar : SOEvent {
    [Header("Random War")]
    [SerializeField] private bool toPlayer;

    private Faction from_faction;
    private Faction to;

    public override string GetDescription() {
        return $"{from_faction.Name} has declared war on {to.Name}!";
    }

    private bool CheckVisible(Faction faction) {
        foreach (Plot plot in RunManager.instance.GetAllFactionPlots(faction)) if (plot.visibleToPlayer) return true;
        return false;
    }

    public override bool IsValid() {
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) if (faction.atWarWith.Values.Contains(false) && CheckVisible(faction)) return true;
        return false;
    }

    public override void Setup() {
        from_faction = Utils.Choice(
            (
                from faction in GameManager.instance.Game.BaseFactions 
                where faction.FactionType == GameManager.FactionTypes.Kingdom && faction.atWarWith.Values.Contains(false)
                select faction
            ).ToArray()
        );

        if (toPlayer) {
            to = GameManager.instance.Game.PlayerFaction;
        } else {
            List<Faction> valid_to = (
                from faction in GameManager.instance.Game.BaseFactions 
                where faction.FactionType == GameManager.FactionTypes.Kingdom && from_faction.atWarWith[faction] == false
                select faction
            ).ToList();
            valid_to.Remove(from_faction);
            to = Utils.Choice(valid_to);
        }
    }

    public override void Event() {
        from_faction.DeclareWar(to);
    }
}
