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

    public Game(
        Vector2Int terrain_size,
        Dictionary<SOPlot, int> plot_generation_data
    ) {
        TerrainSize = terrain_size;
        GenerateFactions();
        GenerateBaseTerrain(plot_generation_data);
        PlaceCastle();
        PlaceBarbCamps(1);
        PlayerFaction = new(GameManager.FactionTypes.Kingdom, "Player Kingdom", "Player Name");
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
                int max = 0;
                foreach (int chance in plot_generation_data.Values) max += chance;
                int rand = GameManager.Random.Next(1, max);

                int threshold = 0;
                foreach (SOPlot plot in plot_generation_data.Keys) {
                    threshold += plot_generation_data[plot];
                    if (rand < threshold) {
                        BaseTerrain[y][x] = plot;
                        break;
                    }
                }
            }
        }
    }

    private void PlaceCastle() {
        for (int x = TerrainSize.x / 2; x < TerrainSize.x; x++) {
            for (int y = TerrainSize.y / 2; y < TerrainSize.y; y++) {
                if (BaseTerrain[y][x].Prefab.GetComponent<Plot>().GetCanPlaceObject()) {
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
            BaseTerrain[y][x].Prefab.GetComponent<Plot>().GetCanPlaceObject()
        ) {
            BarbCamps.Add(new Vector2Int(x, y));
        } else {
            PlaceCamp();
        }
    }
}
