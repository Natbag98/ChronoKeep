using UnityEngine;
using System.Collections.Generic;

public class RunManager : MonoBehaviour {
    public static RunManager instance;

    [Header("References")]
    [SerializeField] private Transform plotContainer;

    private Plot[][] plotArray;

    public Plot[][] GetPlotArray() { return plotArray; }

    public Plot GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes placed_object) {
        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                if (plot.placedObjectType == placed_object) return plot;
            }
        }
        return null;
    }

    public List<Plot> GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes placed_object) {
        List<Plot> plots = new();
        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                if (plot.placedObjectType == placed_object) plots.Add(plot);
            }
        }
        if (plots.Count == 0) return null;
        return plots;
    }

    private void InstantiatePlots() {
        Game game = GameManager.instance.Game;
        plotArray = Utils.CreateJaggedArray<Plot[][]>(game.TerrainSize.x, game.TerrainSize.y);
        for (int x = 0; x < game.TerrainSize.x; x++) {
            for (int y = 0; y < game.TerrainSize.y; y++) {
                GameObject new_plot = Instantiate(
                    game.BaseTerrain[y][x].Prefab,
                    new Vector3Int(x - game.TerrainSize.x / 2, 0, y - game.TerrainSize.y / 2),
                    Quaternion.identity,
                    plotContainer
                );
                plotArray[y][x] = new_plot.GetComponent<Plot>();
            }
        }

        for (int y = 0; y < game.TerrainSize.y; y++) {
            for (int x = 0; x < game.TerrainSize.x; x++) {
                Plot[] neighbours = new Plot[4];
                if (y != 0) neighbours[Plot.neighbourUp] = plotArray[y - 1][x];
                if (y != game.TerrainSize.y - 1) neighbours[Plot.neighbourDown] = plotArray[y + 1][x];
                if (x != 0) neighbours[Plot.neighbourLeft] = plotArray[y][x - 1];
                if (x != game.TerrainSize.x - 1) neighbours[Plot.neighbourRight] = plotArray[y][x + 1];
                plotArray[y][x].SetNeighbours(neighbours);
            }
        }

        plotArray[game.CastleLocation.y][game.CastleLocation.x].PlaceObject(GameManager.instance.Castle);
        foreach (Vector2Int location in game.BarbCamps) {
            plotArray[location.y][location.x].PlaceObject(GameManager.instance.BarbCamp);
        }
    }

    private void Start() {
        instance = this;
        InstantiatePlots();
    }
}
