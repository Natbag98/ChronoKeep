using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

public class RunManager : MonoBehaviour, ISaveSystem {
    public static RunManager instance;

    [Header("References")]
    [SerializeField] private Transform plotContainer;
    public Transform characterContainer;
    public Transform projectileContainer;

    private Plot[][] plotArray;
    private int score;
    private float scoreMult = 1;
    private float skillMult = 1;
    [HideInInspector] public bool paused = false;
    [HideInInspector] public List<Mod> globalMods = new();
    [HideInInspector] public float simSpeed = 1f;

    private bool test = true;
    [SerializeField] private SOPlaceableObject[] testPlacement;
    public Mod[] testMods;

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
    public Plot GetFirstPlotWithPlacedObject(GameManager.PlaceableObjectTypes placed_object, Faction faction=null) {
        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                if (plot.placedObjectType == GameManager.PlaceableObjectTypes.Feature) continue;
                if (faction == null) {
                    if (plot.placedObjectType == placed_object) return plot;
                } else {
                    if (plot.placedObjectType == placed_object && plot.faction == faction) return plot;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Gets a list of plots with the given object placed.
    /// </summary>
    /// <param name="placed_object">The object to check for.</param>
    /// <returns>The list of plots with the placed object.</returns>
    public List<Plot> GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes placed_object, Faction faction=null) {
        List<Plot> plots = new();
        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                if (plot.placedObjectType == GameManager.PlaceableObjectTypes.Feature) continue;
                if (faction == null) {
                    if (plot.placedObjectType == placed_object) plots.Add(plot);
                } else {
                    if (plot.placedObjectType == placed_object && plot.faction == faction) plots.Add(plot);
                }
            }
        }
        if (plots.Count == 0) return null;
        return plots;
    }

    /// <summary>
    /// Gets all the plots with a placed object belonging to a given faction
    /// </summary>
    public List<Plot> GetAllPlotsWithFactionObjects(Faction faction) {
        return (
            from placeable_object_type
            in Utils.GetEnumValues<GameManager.PlaceableObjectTypes>()
            where GetAllPlotsWithPlacedObject(placeable_object_type, faction) != null
            select GetAllPlotsWithPlacedObject(placeable_object_type, faction)
        ).SelectMany(x => x).ToList();
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
                InstantiatePlot(x, y, GameManager.instance.Game.BaseTerrain[y][x]);
            }
        }

        SetPlotNeighbours();

        foreach (Game.BaseObjectInfo object_info in game.baseObjectInfo) {
            Plot plot = plotArray[object_info.location.y][object_info.location.x];
            if (!plot.GetCanPlaceObject()) {
                Destroy(plot.gameObject);
                InstantiatePlot(object_info.location.x, object_info.location.y, GameManager.instance.Plains);
            }
            SetPlotNeighbours();
            plotArray[object_info.location.y][object_info.location.x].PlaceObject(object_info.base_object, object_info.faction);
        }
    }

    public void SetPlotNeighbours() {
        Game game = GameManager.instance.Game;
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
    }

    public Plot InstantiatePlot(int x, int y, SOPlot plotSO) {
        Plot new_plot = Instantiate(
            plotSO.prefab,
            new Vector3Int(x - GameManager.instance.Game.TerrainSize.x / 2, 0, y - GameManager.instance.Game.TerrainSize.y / 2),
            Quaternion.identity,
            plotContainer
        ).GetComponent<Plot>();
        new_plot.plotSO = plotSO;
        plotArray[y][x] = new_plot;
        return new_plot;
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
        paused = true;
        CameraSystem.instance.cameraBlocked = true;
        MainSceneUIManager.instance.pauseMenu.SetActive(true);
    }

    public void Unpause() {
        paused = false;
        CameraSystem.instance.cameraBlocked = false;
        MainSceneUIManager.instance.pauseMenu.SetActive(false);
    }

    public void AddScore(int amount) {
        score += Mathf.FloorToInt(amount * scoreMult);
    }

    public void GameOver() {
        GameManager.instance.scoreLastRun = score;
        GameManager.instance.skillLastRun = Mathf.FloorToInt(score / 10 * skillMult);
        GameManager.instance.Game.skillPoints += GameManager.instance.skillLastRun;
        if (GameManager.instance.Game.firstRun) {
            SceneManager.LoadScene("CutScene");
            GameManager.instance.nextScene = "PostRunScene";
            GameManager.instance.storyDisplay = 1;
            GameManager.instance.Game.firstRun = false;
        } else {
            SceneManager.LoadScene("PostRunScene");
        }
    }

    private void Start() {
        instance = this;

        InstantiatePlots();
        GameManager.instance.Game.PlayerFaction.RunStart();
        if (GameManager.instance.Game.firstRun) TutorialManager.instance.StartTutorial();
        foreach (Faction faction in GameManager.instance.Game.BaseFactions) faction.RunStart();
        foreach (SOPerk perk in GameManager.instance.Game.perksUnlockTracker.GetAllUnlocked()) {
            scoreMult += perk.scoreMultIncrease;
            skillMult += perk.skillMultIncrease;
            globalMods.AddRange(perk.modsToApply);
        }

        if (test) {
            test = false;
            foreach (SOPlaceableObject object_to_place in testPlacement) MainSceneUIManager.instance.PlaceInventoryItem(object_to_place);
        }
    }

    private void Update() {
        foreach (IModable modable in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IModable>()) {
            foreach (Mod mod in globalMods) modable.AddMod(mod);
        }

        if (GetAllPlotsWithPlacedObject(GameManager.PlaceableObjectTypes.Castle, GameManager.instance.Game.PlayerFaction) == null) {
            GameOver();
        }
    }

    public void SaveData(GameData data) {
        data.runData.globalMods = globalMods;
        data.runData.score = score;

        data.runData.plotData = Utils.CreateJaggedArray<PlotData[][]>(GameManager.instance.Game.TerrainSize.x, GameManager.instance.Game.TerrainSize.y);
        for (int x = 0; x < GameManager.instance.Game.TerrainSize.x; x++) {
            for (int y = 0; y < GameManager.instance.Game.TerrainSize.y; y++) {
                Plot plot = plotArray[y][x];
                data.runData.plotData[y][x] = new() {
                    plotSO = plot.plotSO.name,
                    faction = plot.faction?.Name
                };

                if (plot.placedObjectSO) {
                    data.runData.plotData[y][x].placedObject = new() {
                        placeableObjectSO = plot.GetComponentInChildren<PlaceableObject>().placeableObjectSO.name,
                        health = plot.GetComponentInChildren<PlaceableObject>().health
                    };
                }
            }
        }

        foreach (Faction faction in GameManager.instance.Game.BaseFactions.Append(GameManager.instance.Game.PlayerFaction)) {
            data.runData.factionsWars.Add(faction.Name, new());
            foreach (Faction war_faction in faction.atWarWith.Keys) {
                data.runData.factionsWars[faction.Name].Add(war_faction.Name, faction.atWarWith[war_faction]);
            }
        }
    }

    public void LoadData(GameData data) {
        globalMods = data.runData.globalMods;
        score = data.runData.score;

        foreach (Plot[] row in plotArray) {
            foreach (Plot plot in row) {
                Destroy(plot.gameObject);
            }
        }

        plotArray = Utils.CreateJaggedArray<Plot[][]>(data.terrainSize.x, data.terrainSize.y);
        for (int x = 0; x < data.terrainSize.x; x++) {
            for (int y = 0; y < data.terrainSize.y; y++) {
                Plot new_plot = InstantiatePlot(x, y, Utils.GetAsset<SOPlot>(data.runData.plotData[y][x].plotSO));

                if (data.runData.plotData[y][x].faction == null) {
                    new_plot.faction = null;
                } else {
                    new_plot.faction = GameManager.instance.Game.GetFactionByName(data.runData.plotData[y][x].faction);
                }

                if (data.runData.plotData[y][x].placedObject != null) {
                    PlaceableObject new_object = new_plot.PlaceObject(Utils.GetAsset<SOPlaceableObject>(data.runData.plotData[y][x].placedObject.placeableObjectSO));
                    new_object.loaded = true;
                    new_object.health = data.runData.plotData[y][x].placedObject.health;
                }
            }
        }
        SetPlotNeighbours();

        foreach (string faction in data.runData.factionsWars.Keys) {
            GameManager.instance.Game.GetFactionByName(faction).atWarWith = new();
            foreach (string war_faction in data.runData.factionsWars[faction].Keys) {
                GameManager.instance.Game.GetFactionByName(faction).atWarWith.Add(
                    GameManager.instance.Game.GetFactionByName(war_faction),
                    data.runData.factionsWars[faction][war_faction]
                );
            }
        }
    }
}
