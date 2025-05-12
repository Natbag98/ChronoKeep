using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWar", menuName = "Event/RandomWar")]
public class RandomWar : SOEvent {
    [Header("Random War")]
    [SerializeField] private bool toPlayer;

    private Faction from;
    private Faction to;

    public override string GetDescription() {
        return $"{from.Name} has declared war on {to.Name}!";
    }

    public override void Setup() {
        from = Utils.Choice(
            (
                from faction in GameManager.instance.Game.BaseFactions 
                where faction.FactionType == GameManager.FactionTypes.Kingdom
                select faction
            ).ToArray()
        );

        if (toPlayer) {
            to = GameManager.instance.Game.PlayerFaction;
        } else {
            List<Faction> valid_to = (
                from faction in GameManager.instance.Game.BaseFactions 
                where faction.FactionType == GameManager.FactionTypes.Kingdom
                select faction
            ).ToList();
            valid_to.Remove(from);
            to = Utils.Choice(valid_to);
        }
    }

    public override void Event() {
        from.DeclareWar(to);
    }
}
