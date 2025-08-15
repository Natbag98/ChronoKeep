using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SendEnvoy", menuName = "Event/SendEnvoy")]
public class SendEnvoy : SOEvent {
    private Faction faction;

    public override string GetDescription() {
        return $"King {faction.Ruler} of {faction.Name} has sent you an envoy. All your land is now revealed to eachother.";
    }

    public override bool IsValid() {
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            if (
                faction.FactionType == GameManager.FactionTypes.Kingdom &&
                RunManager.instance.GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes.Castle, faction).visibleToPlayer &&
                !faction.atWarWith[GameManager.instance.Game.PlayerFaction]
            ) {
                return true;
            }
        }
        return false;
    }

    public override void Setup() {
        List<Faction> factions = new();
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            if (faction.FactionType == GameManager.FactionTypes.Kingdom) {
                foreach (Plot plot in RunManager.instance.GetAllFactionPlots(faction)) {
                    if (plot.visibleToPlayer) factions.Add(faction);
                }
            }
        }
        faction = Utils.Choice(factions);
    }

    public override void Event() {
        foreach (Plot plot in RunManager.instance.GetAllFactionPlots(faction)) {
            foreach (Plot neighbour in plot.GetNeighbours(square: true, include_self: true)) neighbour.SetVisibleToPlayer(true);
        }
    }
}
