using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Faction {
    public GameManager.FactionTypes FactionType { private set; get; }
    public string Name { private set; get; }
    public string Ruler {private set; get; }

    public Faction(
        GameManager.FactionTypes? faction_type=null,
        string name=null,
        string ruler=null
    ) {
        if (faction_type != null) {
            FactionType = (GameManager.FactionTypes)faction_type;
        } else {
            FactionType = (GameManager.FactionTypes)GameManager.Random.Next(Enum.GetValues(typeof(GameManager.FactionTypes)).Length);
        }

        if (name != null) {
            Name = name;
        } else {
            if (faction_type == GameManager.FactionTypes.Kingdom) {
                Name = Utils.Choice(GameManager.instance.TextData.Data["kingdom_names"]);
            } else if (faction_type == GameManager.FactionTypes.BarbarianClan) {
                Name = $"The {Utils.Choice(GameManager.instance.TextData.Data["clan_names"])} Clans";
            }
        }

        if (ruler != null) {
            Ruler = ruler;
        } else {
            Ruler = $"{Utils.Choice(GameManager.instance.TextData.Data["first_names"])} {Utils.Choice(GameManager.instance.TextData.Data["last_names"])}";
        }
    }

    public void OnWaveStart(int base_power) {
        List<Plot> plots_with_spawners = RunManager.instance.GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Spawner);
        foreach (Plot plot in plots_with_spawners) {
            if (plot.faction == this) plot.GetComponentInChildren<Spawner>().powerRemaining = base_power;
        }
    }
}
