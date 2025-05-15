using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        Dictionary<SOPlot, int> plot_generation_data,
        string playerName,
        string kingdomName
    ) {
        usedFactionNames = new();
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.ArcherTower);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.Castle);
        placeableObjectsUnlockTracker.UpdateUnlocked(GameManager.instance.BarbCamp);

        PlayerFaction = new(this, GameManager.FactionTypes.Kingdom, kingdomName, playerName);
        resources = GameManager.instance.startingResources.GetDict();
        TerrainSize = terrain_size;
        GenerateBaseTerrain(plot_generation_data);
        GenerateFactions();
        PlaceCastle();
        PlaceBarbCamps(1);
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

    public void ResetManpower() {
        resources[GameManager.Resources.ManPower] = 0;
    }

    private void GenerateFactions() {
        Vector2Int[] castle_locations = {
            new(3, TerrainSize.y / 2),
            new(TerrainSize.x - 3, TerrainSize.y / 2),
            new(TerrainSize.x / 2, TerrainSize.y - 3),
            new(TerrainSize.x / 2, 3),
        };

        for (int i = 0; i < 4; i++) {
            Faction faction = new(this, GameManager.FactionTypes.Kingdom);
            BaseFactions.Add(faction);

            PlaceObject(
                GameManager.instance.Castle,
                faction,
                baseObjectInfo,
                castle_locations[i],
                constraint_max_distance: 2
            );

            PlaceObject(
                GameManager.instance.BarbCamp,
                faction,
                baseObjectInfo,
                castle_locations[i],
                constraint_max_distance: 4
            );

            PlaceObject(
                GameManager.instance.ArcherTower,
                faction,
                baseObjectInfo,
                castle_locations[i],
                constraint_max_distance: 4
            );
        }
        BaseFactions.Add(new(this, GameManager.FactionTypes.BarbarianClan));
    }

    private void GenerateBaseTerrain(Dictionary<SOPlot, int> plot_generation_data) {
        BaseTerrain = CreateJaggedArray<SOPlot[][]>(TerrainSize.x, TerrainSize.y);
        for (int x = 0; x < TerrainSize.x; x++) {
            for (int y = 0; y < TerrainSize.y; y++) {
                BaseTerrain[y][x] = Choice(plot_generation_data);
            }
        }
    }

    private void PlaceCastle() {
        for (int x = TerrainSize.x / 2; x < TerrainSize.x; x++) {
            for (int y = TerrainSize.y / 2; y < TerrainSize.y; y++) {
                if (BaseTerrain[y][x].prefab.GetComponent<Plot>().GetCanPlaceObject() && BaseTerrain[y][x].prefab.GetComponent<Plot>().walkable) {
                    baseObjectInfo.Add(new BaseObjectInfo{
                        location = new(x, y),
                        base_object = GameManager.instance.Castle,
                        faction = PlayerFaction
                    });
                    playerCastleLocation = new(x, y);
                    return;
                }
            }
        }
    }

    private void PlaceBarbCamps(int count) {
        for (int i = 0; i < count; i++) PlaceObject(
            GameManager.instance.BarbCamp,
            BaseFactions[^1],
            baseObjectInfo,
            playerCastleLocation,
            GameManager.instance.MinBarbGenerationDistance,
            GameManager.instance.MaxBarbGenerationDistance
        );
    }

    private void PlaceObject(
        SOPlaceableObject object_to_place,
        Faction faction,
        List<BaseObjectInfo> list_to_place,
        Vector2Int? constraint_location_input=null,
        int constraint_min_distance=0,
        int constraint_max_distance=1000
    ) {
        Vector2Int constraint_location = Vector2Int.zero;
        if (constraint_location_input != null) {
            constraint_location = (Vector2Int)constraint_location_input;
        }
        
        int x = Mathf.Clamp(GenerateNumberAroundCenter(constraint_location.x, constraint_min_distance, constraint_max_distance), 0, TerrainSize.x - 1);
        int y = Mathf.Clamp(GenerateNumberAroundCenter(constraint_location.y, constraint_min_distance, constraint_max_distance), 0, TerrainSize.y - 1);

        if (
            !(from base_object_info in list_to_place select base_object_info.location).Contains(new Vector2Int(x, y)) &&
            BaseTerrain[x][y].prefab.GetComponent<Plot>().GetCanPlaceObject(object_to_place)
        ) {
            list_to_place.Add(new BaseObjectInfo{
                location = new(x, y),
                base_object = object_to_place,
                faction = faction
            });
        } else {
            PlaceObject(object_to_place, faction, list_to_place, constraint_location, constraint_min_distance, constraint_max_distance);
        }
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
        foreach (string object_ in data.characterUnlockTracker.disovered.Keys) {
            characterUnlockTracker.disovered[GetAsset<SOCharacter>(object_)] = data.characterUnlockTracker.disovered[object_];
        }
        foreach (string object_ in data.characterUnlockTracker.unlocked.Keys) {
            characterUnlockTracker.unlocked[GetAsset<SOCharacter>(object_)] = data.characterUnlockTracker.unlocked[object_];
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
