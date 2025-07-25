using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AggroWar", menuName = "Event/AggroWar")]
public class AggroWar : SOEvent {
    private Faction aggroFaction;

    public override string GetDescription() {
        return $"{aggroFaction.Name} is declaring war on {GameManager.instance.Game.PlayerFaction.Name}";
    }

    public override void Setup() {
        int aggro = 0;
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            if (faction.aggro > aggro) aggroFaction = faction;
        }
    }

    public override void Event() {
        aggroFaction.DeclareWar(GameManager.instance.Game.PlayerFaction);
    }
}
