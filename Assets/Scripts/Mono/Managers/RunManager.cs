using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform plotContainer;
    public Transform characterContainer;
    public Transform projectileContainer;

    private Plot[][] plotArray;
    [HideInInspector] public List<Faction> factions = new();
    [HideInInspector] public Faction playerFaction;
    [HideInInspector] public bool paused = false;

    private bool test = true;
    [SerializeField] private SOPlaceableObject[] testPlacement;

    public Plot[][] GetPlotArray() { return plotArray; }
    public List<Plot> GetPlotList() {
        List<Plot> to_return = new();
        foreach (Plot[] row in plotArray) to_return.AddRange(row);
        return to_return;
    }

    /// <summary>
    /// Get the first plot with the given object placed.
    /// </summary>
    /// <param name="placed_object">The object to check for.</param>
    /// <returns>The plot with the placed object.</returns>
    public Plot GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes placed_object) {
        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                if (plot.placedObjectType == placed_object) return plot;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets a list of plots with the given object placed.
    /// </summary>
    /// <param name="placed_object">The object to check for.</param>
    /// <returns>The list of plots with the placed object.</returns>
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

    /// <summary>
    /// Instantiates the plots included in the Game class into the scene.
    /// Should be called at the start of each run.
    /// </summary>
    private void InstantiatePlots() {
        Game game = GameManager.instance.Game;
        plotArray = Utils.CreateJaggedArray<Plot[][]>(game.TerrainSize.x, game.TerrainSize.y);
        for (int x = 0; x < game.TerrainSize.x; x++) {
            for (int y = 0; y < game.TerrainSize.y; y++) {
                Plot new_plot = Instantiate(
                    game.BaseTerrain[y][x].Prefab,
                    new Vector3Int(x - game.TerrainSize.x / 2, 0, y - game.TerrainSize.y / 2),
                    Quaternion.identity,
                    plotContainer
                ).GetComponent<Plot>();
                new_plot.plotType = game.BaseTerrain[y][x].plotType;
                plotArray[y][x] = new_plot;
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

        plotArray[game.CastleLocation.y][game.CastleLocation.x].PlaceObject(GameManager.instance.Castle, GameManager.instance.Game.PlayerFaction);
        foreach (Vector2Int location in game.BarbCamps) {
            plotArray[location.y][location.x].PlaceObject(GameManager.instance.BarbCamp, GameManager.instance.Game.BaseFactions[^1]);
        }
    }

    public void PlaceRandomObject(SOPlaceableObject object_to_place, Faction faction, bool no_mans_land=false) {
        List<Plot> plots = new();
        if (!no_mans_land) {
            foreach (Plot plot in GetPlotList()) if (faction == plot.faction && plot.GetCanPlaceObject()) plots.Add(plot);
        } else {
            foreach (Plot plot in GetPlotList()) if (faction == plot.faction || plot.faction == null && plot.GetCanPlaceObject()) plots.Add(plot);
        }
        Utils.Choice(plots).PlaceObject(object_to_place, faction);
    }

    public void Pause() {
        Time.timeScale = 0;
        paused = true;
        Utils.GetManager<MainSceneUIManager>().pauseMenu.SetActive(true);
    }

    public void Unpause() {
        Time.timeScale = 1;
        paused = false;
        Utils.GetManager<MainSceneUIManager>().pauseMenu.SetActive(false);
    }

    public void GameOver() {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void Start() {
        InstantiatePlots();
        factions.AddRange(GameManager.instance.Game.BaseFactions);
        playerFaction = GameManager.instance.Game.PlayerFaction;

        if (test) {
            test = false;
            foreach (SOPlaceableObject object_to_place in testPlacement) Utils.GetManager<MainSceneUIManager>().PlaceInventoryItem(object_to_place);
        }
    }

    private void Update() {
        if (
            GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Castle) == null ||
            GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Spawner) == null
        ) {
            GameOver();
        }
    }
}
