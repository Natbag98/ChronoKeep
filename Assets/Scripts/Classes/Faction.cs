using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Faction {
    public GameManager.FactionTypes FactionType { private set; get; }
    public string Name { private set; get; }
    public string Ruler { private set; get; }
    public Color Color { private set; get; }

    public Dictionary<Faction, bool> atWarWith = new();
    public int aggro = 0;
    public int peaceCost;
    public int envoyCost;

    public Faction(
        Game game,
        GameManager.FactionTypes? faction_type=null,
        string name=null,
        string ruler=null,
        Color? color=null
    ) {
        if (string.IsNullOrWhiteSpace(name)) name = null;
        if (string.IsNullOrWhiteSpace(ruler)) ruler = null;
        
        if (faction_type != null) {
            FactionType = (GameManager.FactionTypes)faction_type;
        } else {
            FactionType = (GameManager.FactionTypes)GameManager.Random.Next(Enum.GetValues(typeof(GameManager.FactionTypes)).Length);
        }

        if (name != null) {
            Name = name;
        } else {
            while (true) {
                if (faction_type == GameManager.FactionTypes.Kingdom) {
                    Name = Utils.Choice(GameManager.instance.TextData.Data["kingdom_names"]);
                } else if (faction_type == GameManager.FactionTypes.BarbarianClan) {
                    Name = $"The {Utils.Choice(GameManager.instance.TextData.Data["clan_names"])} Clans";
                }

                if (!game.usedFactionNames.Contains(Name)) {
                    game.usedFactionNames.Add(Name);
                    break;
                }
            }
        }

        if (ruler != null) {
            Ruler = ruler;
        } else {
            Ruler = $"{Utils.Choice(GameManager.instance.TextData.Data["first_names"])} {Utils.Choice(GameManager.instance.TextData.Data["last_names"])}";
        }

        if (color != null) {
            Color = (Color)color;
        } else {
            Color = new(
                (float)GameManager.Random.NextDouble(),
                (float)GameManager.Random.NextDouble(),
                (float)GameManager.Random.NextDouble(),
                GameManager.instance.alpha / 255f
            );
        }

        peaceCost = GameManager.Random.Next(15 + GameManager.instance.difficulty.base_peace_cost, 25 + GameManager.instance.difficulty.base_peace_cost);
        envoyCost = GameManager.Random.Next(3 + GameManager.instance.difficulty.base_envoy_cost, 6 + GameManager.instance.difficulty.base_envoy_cost);
    }

    public void OnWaveEnd() {
        if (FactionType == GameManager.FactionTypes.Kingdom){
            int max_aggro = 0;
            foreach (Faction faction in GameManager.instance.Game.BaseFactions) if (faction.aggro > max_aggro) max_aggro = faction.aggro;
            if (max_aggro == aggro && GameManager.Random.Next(100) > aggro) EventManager.instance.eventList.Add(Utils.GetAsset<AggroWar>("AggroWar"));
        }
    }

    public void OnWaveStart(int base_power) {
        List<Plot> plots_with_spawners = RunManager.instance.GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Spawner);
        foreach (Plot plot in plots_with_spawners) {
            if (
                plot.faction == this &&
                (
                    from faction in atWarWith.Keys
                    where faction.FactionType == GameManager.FactionTypes.Kingdom && atWarWith[faction]
                    select faction
                ).ToList().Count != 0
            ) {
                plot.GetComponentInChildren<Spawner>().SpawnHostileWave((int)Math.Ceiling(base_power * (int)Math.Ceiling(GetWarCount() / 2f) * GameManager.instance.difficulty.powerMult));
            }
        }
    }

    public int GetWarCount() {
        return (from faction in atWarWith.Values where faction select faction).ToArray().Length;
    }

    public Faction[] GetAtWar() {
        return (from faction in atWarWith where faction.Value select faction.Key).ToArray();
    }

    public void DeclareWar(Faction faction) {
        if (faction == this) return;
        atWarWith[faction] = true;
        faction.atWarWith[this] = true;
    }

    public void MakePeace(Faction faction) {
        if (faction == this) return;
        atWarWith[faction] = false;
        faction.atWarWith[this] = false;
    }

    public void RunStart() {
        atWarWith = new() {
            { GameManager.instance.Game.PlayerFaction, false }
        };
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) {
            atWarWith.Add(faction, false);
        }

        if (FactionType == GameManager.FactionTypes.BarbarianClan) {
            foreach (Faction faction in atWarWith.Keys.ToList()) faction.DeclareWar(this); 
        }

        // Test
        // foreach (Faction faction in atWarWith.Keys.ToList()) {
        //     faction.DeclareWar(this);
        // }
    }
}
