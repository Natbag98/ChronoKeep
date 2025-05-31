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
        Dictionary<SOPlot, float> plot_generation_data,
        string playerName,
        string kingdomName,
        int non_barb_faction_count
    ) {
        usedFactionNames = new();
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.ArcherTower);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.Castle);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.BarbCamp);

        PlayerFaction = new(this, GameManager.FactionTypes.Kingdom, kingdomName, playerName);
        resources = GameManager.instance.startingResources.GetDict();
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

    public void AddResources(Dictionary<GameManager.Resources, int> resource_dict) {
        foreach (GameManager.Resources resource in resource_dict.Keys) {
            resources[resource] += resource_dict[resource];
        }
    }

    public bool SpendResources(Dictionary<GameManager.Resources, int> resource_dict) {
        if (!CanSpendResources(resource_dict)) return false;
        foreach (GameManager.Resources resource in resource_dict.Keys) SpendResources(resource, resource_dict[resource]);
        return true;
    }

    private void GenerateFactions(int faction_count, int barb_count) {
        List<Vector2Int> castle_locations = new() {
            PlaceObject(baseObjectInfo, GameManager.instance.Castle, PlayerFaction)
        };

        for (int i = 0; i < faction_count; i++) {
            Faction faction = new(this, GameManager.FactionTypes.Kingdom);
            BaseFactions.Add(faction);
            Vector2Int castle_location = PlaceObject(baseObjectInfo, GameManager.instance.Castle, faction, avoid_locations: castle_locations, avoid_by: 10);
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

    private void GenerateBaseTerrain(Dictionary<SOPlot, float> plot_generation_data) {
        int xOffset = GameManager.Random.Next(GameManager.instance.maxOffset);
        int yOffset = GameManager.Random.Next(GameManager.instance.maxOffset);
        float[][] falloff_map = GenerateFalloffMap(TerrainSize);
        float[][] heightmap = CreateJaggedArray<float[][]>(TerrainSize.x, TerrainSize.y);
        for (int y = 0; y < TerrainSize.y; y++) {
            for (int x = 0; x < TerrainSize.x; x++) {
                float xCoord = (float)x / TerrainSize.x * GameManager.instance.noiseScale;
                float yCoord = (float)y / TerrainSize.y * GameManager.instance.noiseScale;
                float perlinValue = Mathf.PerlinNoise((xCoord + xOffset) / GameManager.instance.maxOffset, (yCoord + yOffset) / GameManager.instance.maxOffset);
                heightmap[y][x] = Mathf.Clamp01(perlinValue * GameManager.instance.noiseStrength - falloff_map[y][x] * GameManager.instance.falloffStrength);
            }
        }

        BaseTerrain = CreateJaggedArray<SOPlot[][]>(TerrainSize.x, TerrainSize.y);
        plot_generation_data = plot_generation_data.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        for (int x = 0; x < TerrainSize.x; x++) {
            for (int y = 0; y < TerrainSize.y; y++) {
                foreach (var pair in plot_generation_data) {
                    if (heightmap[y][x] <= pair.Value) {
                        SOPlot plot_to_place = pair.Key;
                        foreach (ReplacePlot replace in GameManager.instance.replacePlots) {
                            if (replace.Replace(plot_to_place)) {
                                plot_to_place = replace.plot;
                                break;
                            }
                        }
                        BaseTerrain[y][x] = plot_to_place;
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
