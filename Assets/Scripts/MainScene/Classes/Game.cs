using System.Collections.Generic;
using UnityEngine;

public class Game {
    public Vector2Int TerrainSize { private set; get; }
    public SOPlot[][] BaseTerrain { private set; get; }

    public Game(
        Vector2Int terrain_size,
        Dictionary<SOPlot, int> plot_generation_data
    ) {
        TerrainSize = terrain_size;
        GenerateBaseTerrain(plot_generation_data);
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
}
