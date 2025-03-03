using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game {
    public Vector2Int TerrainSize { private set; get; }
    public SOPlot[][] BaseTerrain { private set; get; }

    public Vector2Int CastleLocation { private set; get; }
    public List<Vector2Int> BarbCamps { private set; get; } = new();

    public List<Faction> BaseFactions { private set; get; } = new();
    public Faction PlayerFaction { private set; get; }

    private Dictionary<GameManager.Resources, int> resources = new();
    public Dictionary<GameManager.Resources, int> GetResources() { return resources; }

    public Game(
        Vector2Int terrain_size,
        Dictionary<SOPlot, int> plot_generation_data,
        string playerName,
        string kingdomName
    ) {
        foreach (GameManager.Resources resource in Enum.GetValues(typeof(GameManager.Resources)).Cast<GameManager.Resources>()) resources.Add(resource, 5);
        TerrainSize = terrain_size;
        GenerateFactions();
        GenerateBaseTerrain(plot_generation_data);
        PlaceCastle();
        PlaceBarbCamps(1);
        PlayerFaction = new(GameManager.FactionTypes.Kingdom, kingdomName, playerName);
    }

    public bool CanSpendResources(GameManager.Resources resource, int amount) {
        return amount < resources[resource];
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
        for (int i = 0; i < 4; i++) {
            BaseFactions.Add(new(GameManager.FactionTypes.Kingdom));
        }
        BaseFactions.Add(new(GameManager.FactionTypes.BarbarianClan));
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
                    CastleLocation = new Vector2Int(x, y);
                    return;
                }
            }
        }
    }

    private void PlaceBarbCamps(int count) {
        for (int i = 0; i < count; i++) PlaceCamp();
    }

    private void PlaceCamp() {
        SOPlot[] row = Utils.Choice(BaseTerrain); int y = Array.IndexOf(BaseTerrain, row);
        SOPlot plot = Utils.Choice(row); int x = Array.IndexOf(row, plot);

        if (
            Vector2Int.Distance(new Vector2Int(x, y), CastleLocation) >= GameManager.instance.MinBarbGenerationDistance &&
            !BarbCamps.Contains(new Vector2Int(x, y)) &&
            BaseTerrain[y][x].prefab.GetComponent<Plot>().GetCanPlaceObject()
        ) {
            BarbCamps.Add(new Vector2Int(x, y));
        } else {
            PlaceCamp();
        }
    }

    public void DebugUpdate() {
        // Debug game logs should go here
    }
}
