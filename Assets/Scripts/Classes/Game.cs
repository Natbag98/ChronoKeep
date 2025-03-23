using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Game {
    public Vector2Int TerrainSize { private set; get; }
    public SOPlot[][] BaseTerrain { private set; get; }

    public List<BaseObjectInfo> baseObjectInfo { private set; get; } = new();

    public List<Faction> BaseFactions { private set; get; } = new();
    public Faction PlayerFaction { private set; get; }

    private Dictionary<GameManager.Resources, int> resources = new();
    public Dictionary<GameManager.Resources, int> GetResources() { return resources; }

    public UnlockTracker<SOPlaceableObject> placeableObjectsUnlockTracker = new();

    public UnlockTracker<SOPerk> perksUnlockTracker = new();

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

    public bool CanSpendResources(GameManager.Resources resource, int amount) {
        return amount <= resources[resource];
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
        foreach (GameManager.Resources resource in resource_dict.Keys) {
            if (!CanSpendResources(resource, resource_dict[resource])) {
                return false;
            }
        }

        foreach (GameManager.Resources resource in resource_dict.Keys) SpendResources(resource, resource_dict[resource]);
        return true;
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
            baseObjectInfo.Add(new BaseObjectInfo{
                location = castle_locations[i],
                base_object = GameManager.instance.Castle,
                faction = faction
            });

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
        BaseTerrain = Utils.CreateJaggedArray<SOPlot[][]>(TerrainSize.x, TerrainSize.y);
        for (int x = 0; x < TerrainSize.x; x++) {
            for (int y = 0; y < TerrainSize.y; y++) {
                BaseTerrain[y][x] = Utils.Choice(plot_generation_data);
            }
        }
    }

    private void PlaceCastle() {
        for (int x = TerrainSize.x / 2; x < TerrainSize.x; x++) {
            for (int y = TerrainSize.y / 2; y < TerrainSize.y; y++) {
                if (BaseTerrain[y][x].prefab.GetComponent<Plot>().GetCanPlaceObject()) {
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
        
        int x = Mathf.Clamp(Utils.GenerateNumberAroundCenter(constraint_location.x, constraint_min_distance, constraint_max_distance), 0, TerrainSize.x - 1);
        int y = Mathf.Clamp(Utils.GenerateNumberAroundCenter(constraint_location.y, constraint_min_distance, constraint_max_distance), 0, TerrainSize.y - 1);

        if (
            !(from base_object_info in list_to_place select base_object_info.location).Contains(new Vector2Int(x, y))
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

    public void DebugUpdate() {
        // Debug game logs should go here
    }
}
