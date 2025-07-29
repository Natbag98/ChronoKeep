using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static Utils;

public class Game {
    public Vector2Int TerrainSize { private set; get; }
    public SOPlot[][] BaseTerrain { private set; get; }

    public List<BaseObjectInfo> baseObjectInfo { private set; get; } = new();

    public List<Faction> BaseFactions { private set; get; } = new();
    public Faction PlayerFaction { private set; get; }

    private Dictionary<GameManager.Resources, int> resources = new();
    public Dictionary<GameManager.Resources, int> GetResources() { return resources; }

    public UnlockTracker<SOPlaceableObject> placeableObjectsUnlockTracker = new();
    public UnlockTracker<SOCharacter> characterUnlockTracker = new();
    public UnlockTracker<SOPerk> perksUnlockTracker = new();
    public int skillPoints = 0;
    public bool firstRun = true;

    private Vector2Int playerCastleLocation;
    public List<string> usedFactionNames;

    public class BaseObjectInfo {
        public Vector2Int location;
        public SOPlaceableObject base_object;
        public Faction faction;
    }

    public Game(
        Vector2Int terrain_size,
        Dictionary<SOGenerationLevel, float> plot_generation_data,
        string playerName,
        string kingdomName,
        int non_barb_faction_count
    ) {
        usedFactionNames = new();
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.ArcherTower);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.Castle);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.BarbCamp);

        PlayerFaction = new(this, GameManager.FactionTypes.Kingdom, kingdomName, playerName);
        resources = GameManager.instance.difficulty.startingResources.GetDict();
        TerrainSize = terrain_size;
        GenerateBaseTerrain(plot_generation_data);
        GenerateFactions(non_barb_faction_count, 2);
    }

    public Faction GetFactionByName(string name) {
        Dictionary<string, Faction> factions = new() { { PlayerFaction.Name, PlayerFaction } };
        foreach (Faction faction in BaseFactions) factions.Add(faction.Name, faction);
        return factions[name];
    }

    public bool CanSpendResources(GameManager.Resources resource, int amount) {
        return amount <= resources[resource];
    }
    public bool CanSpendResources(Dictionary<GameManager.Resources, int> resources) {
        foreach (GameManager.Resources resource in resources.Keys) {
            if (!CanSpendResources(resource, resources[resource])) {
                return false;
            }
        }

        return true;
    }

    public bool SpendResources(GameManager.Resources resource, int amount) {
        if (CanSpendResources(resource, amount)) {
            resources[resource] -= amount;
            return true;
        } else {
            return false;
        }
    }

    public void AddResources(GameManager.Resources resource, int amount) {
        resources[resource] += amount;
    }
    public void AddResources(Dictionary<GameManager.Resources, int> resource_dict) {
        foreach (GameManager.Resources resource in resource_dict.Keys) {
            AddResources(resource, resource_dict[resource]);
        }
    }

    public bool SpendResources(Dictionary<GameManager.Resources, int> resource_dict) {
        if (!CanSpendResources(resource_dict)) return false;
        foreach (GameManager.Resources resource in resource_dict.Keys) SpendResources(resource, resource_dict[resource]);
        return true;
    }

    private void GenerateFactions(int faction_count, int barb_count) {
        List<Vector2Int> castle_locations = new() {
            PlaceObject(
                baseObjectInfo,
                GameManager.instance.Castle,
                PlayerFaction,
                TerrainSize / 2,
                Mathf.Min(TerrainSize.x / 4, TerrainSize.y / 4)
            )
        };

        for (int i = 0; i < faction_count; i++) {
            Faction faction = new(this, GameManager.FactionTypes.Kingdom);
            BaseFactions.Add(faction);
            Vector2Int castle_location = PlaceObject(
                baseObjectInfo,
                GameManager.instance.Castle,
                faction,
                TerrainSize / 2,
                Mathf.Min(TerrainSize.x / 4, TerrainSize.y / 4),
                castle_locations,
                7
            );
            castle_locations.Add(castle_location);
            PlaceObject(baseObjectInfo, GameManager.instance.ArcherTower, faction, castle_location, 4);
            PlaceObject(baseObjectInfo, GameManager.instance.BarbCamp, faction, castle_location, 4);
        }

        BaseFactions.Add(new(this, GameManager.FactionTypes.BarbarianClan));
        PlaceObject(baseObjectInfo, GameManager.instance.BarbCamp, BaseFactions[^1], castle_locations[0], 5);
        for (int i = 0; i < barb_count - 1; i++) {
            PlaceObject(baseObjectInfo, GameManager.instance.BarbCamp, BaseFactions[^1]);
        }
    }

    private SOPlot GetPlot(List<float[][]> heightmaps, int depth, int x, int y, SOGenerationLevel level, float height) {
        if (heightmaps[depth][y][x] <= height) {
            if (level.baseLevel) {
                SOPlot plot_to_place = level.plot;
                foreach (ReplacePlot replace in GameManager.instance.replacePlots) {
                    if (replace.Replace(plot_to_place)) {
                        plot_to_place = replace.plot;
                        break;
                    }
                }
                return plot_to_place;
            } else {
                Dictionary<SOGenerationLevel, float> levels = level.levels.GetDict().OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                foreach (var pair in levels) {
                    SOPlot plot = GetPlot(heightmaps, depth + 1, x, y, pair.Key, pair.Value);
                    if (plot != null) return plot;
                }
            }
        }

        return null;
    }

    private void GenerateBaseTerrain(Dictionary<SOGenerationLevel, float> plot_generation_data) {
        int max_depth = 0;
        foreach (SOGenerationLevel level in plot_generation_data.Keys) if (level.GetMaxDepth(0) > max_depth) max_depth = level.GetMaxDepth(0);
        List<float[][]> heightmaps = (from _ in Enumerable.Range(0, max_depth) select GenerateHeightMap(TerrainSize, false)).ToList();
        heightmaps[0] = GenerateHeightMap(TerrainSize);
        BaseTerrain = CreateJaggedArray<SOPlot[][]>(TerrainSize.x, TerrainSize.y);
        plot_generation_data = plot_generation_data.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        for (int x = 0; x < TerrainSize.x; x++) {
            for (int y = 0; y < TerrainSize.y; y++) {
                foreach (var pair in plot_generation_data) {
                    SOPlot plot = GetPlot(heightmaps, 0, x, y, pair.Key, pair.Value);
                    if (plot != null) {
                        BaseTerrain[y][x] = plot;
                        break;
                    }
                }
            }
        }
    }

    private Vector2Int PlaceObject(
        List<BaseObjectInfo> list_to_place,
        SOPlaceableObject object_to_place,
        Faction faction,
        Vector2Int? center_location=null,
        int? max_dist_from_center=null,
        List<Vector2Int> avoid_locations=null,
        int? avoid_by=null
    ) {
        List<Vector2Int> potential_locations = new();
        for (int x = 0; x < TerrainSize.x; x++) {
            for (int y = 0; y < TerrainSize.y; y++) {
                if (
                    BaseTerrain[y][x].prefab.GetComponent<Plot>().GetCanPlaceObject(object_to_place) &&
                    !(from info in baseObjectInfo select info.location).ToList().Contains(new(x, y))
                ) {
                    if (center_location != null && Vector2Int.Distance((Vector2Int)center_location, new(x, y)) > max_dist_from_center) continue;
                    if (avoid_locations != null) {
                        bool cont = false;
                        foreach (Vector2Int location_to_avoid in avoid_locations) if (Vector2Int.Distance(location_to_avoid, new(x, y)) < avoid_by) cont = true;
                        if (cont) continue;
                    }
                    potential_locations.Add(new(x, y));
                }
            }
        }

        if (potential_locations.Count == 0) return Vector2Int.zero;
        Vector2Int location = Choice(potential_locations);
        list_to_place.Add(new BaseObjectInfo{
            location = location,
            base_object = object_to_place,
            faction = faction
        });
        return location;
    }

    public void LoadData(GameData data) {
        BaseTerrain = CreateJaggedArray<SOPlot[][]>(data.terrainSize.Get().x, data.terrainSize.Get().y);
        for (int x = 0; x < data.terrainSize.Get().x; x++) {
            for (int y = 0; y < data.terrainSize.Get().y; y++) {
                BaseTerrain[y][x] = GetAsset<SOPlot>(data.baseTerrain[y][x]);
            }
        }

        foreach (string perk in data.perkUnlockTracker.disovered.Keys) {
            perksUnlockTracker.disovered[GetAsset<SOPerk>(perk)] = data.perkUnlockTracker.disovered[perk];
        }
        foreach (string perk in data.perkUnlockTracker.unlocked.Keys) {
            perksUnlockTracker.unlocked[GetAsset<SOPerk>(perk)] = data.perkUnlockTracker.unlocked[perk];
        }
        foreach (string object_ in data.placeableObjectUnlockTracker.disovered.Keys) {
            placeableObjectsUnlockTracker.disovered[GetAsset<SOPlaceableObject>(object_)] = data.placeableObjectUnlockTracker.disovered[object_];
        }
        foreach (string object_ in data.placeableObjectUnlockTracker.unlocked.Keys) {
            placeableObjectsUnlockTracker.unlocked[GetAsset<SOPlaceableObject>(object_)] = data.placeableObjectUnlockTracker.unlocked[object_];
        }
        foreach (string character in data.characterUnlockTracker.disovered.Keys) {
            characterUnlockTracker.disovered[GetAsset<SOCharacter>(character)] = data.characterUnlockTracker.disovered[character];
        }
        foreach (string character in data.characterUnlockTracker.unlocked.Keys) {
            characterUnlockTracker.unlocked[GetAsset<SOCharacter>(character)] = data.characterUnlockTracker.unlocked[character];
        }

        PlayerFaction = new(this, data.playerFaction.factionType, data.playerFaction.name, data.playerFaction.rulerName);
        BaseFactions = new();
        foreach (FactionData factionData in data.factionData) {
            BaseFactions.Add(new(this, factionData.factionType, factionData.name, factionData.rulerName));
        }

        Dictionary<string, Faction> factions = new() { { PlayerFaction.Name, PlayerFaction } };
        foreach (Faction faction in BaseFactions) factions.Add(faction.Name, faction);

        baseObjectInfo = new();
        foreach (BaseObjectInfoData baseObjectInfoData in data.baseObjectInfo) {
            baseObjectInfo.Add(
                new() {
                    location = baseObjectInfoData.location.Get(),
                    base_object = GetAsset<SOPlaceableObject>(baseObjectInfoData.baseObject),
                    faction = factions[baseObjectInfoData.faction]
                }
            );
        }

        resources = data.resources;
        skillPoints = data.skill;
        firstRun = data.firstRun;
        TerrainSize = data.terrainSize.Get();
    }

    public void DebugUpdate() {
        // Debug game logs should go here
        Debug.Log($"Skill: {skillPoints}");
    }
}
