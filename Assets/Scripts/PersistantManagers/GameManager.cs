using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveSystem {
    public static GameManager instance;
    public static System.Random Random = new();

    public enum Attributes {
        Health,
        Defense,
        ReloadTime,
        Attack,
        Range,
        CharacterMoveSpeed,
        ProjectileMoveSpeed,
        ExplosionRadius
    }
    public enum PlaceableObjectTypes {
        Castle,
        Tower,
        Spawner,
        CivilianTower
    }
    public enum FactionTypes {
        BarbarianClan,
        Kingdom
    }
    public enum Resources {
        Gold,
        ManPower
    }
    public enum PlotTypes {
        Plains,
        Lake,
        Gold
    }
    public enum PerkTrees {
        KingdomManagement,
        Engineering
    }

    [Header("Static Data")]
    public float PlotMouseOverHeight;
    public float PlotMouseOverSpeed;

    public int MinBarbGenerationDistance;
    public int MaxBarbGenerationDistance;

    public List<string> assetBundles;

    public SOPlaceableObject Castle;
    public SOPlaceableObject BarbCamp;
    public SOPlaceableObject ArcherTower;

    public SOPlot Plains;

    [Header("Plot Generation Data")]
    [SerializeField] private int mapSize;
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> plotGenerationData;

    [Header("Test Data")]
    public Utils.SerializeableDict<Resources, int> startingResources;

    [HideInInspector] public Game Game;
    [HideInInspector] public TextData TextData = new();

    [HideInInspector] public string playerName;
    [HideInInspector] public string kingdomName;

    [HideInInspector] public List<SOPlaceableObject> allSOPlaceableObjects = new();

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (SOPlaceableObject placeable_object in Utils.GetAllAssets<SOPlaceableObject>()) allSOPlaceableObjects.Add(placeable_object);

        if (SceneManager.GetActiveScene().name != "MainMenuScene") {
            Game = new(new(mapSize, mapSize), plotGenerationData.GetDict(), "", "");
        }
    }

    public void NewGame() {
        Game = new(new(mapSize, mapSize), plotGenerationData.GetDict(), kingdomName, playerName);
    }

    void Update() {
        Game.DebugUpdate();
    }

    public void SaveData(GameData data) {
        if (SceneManager.GetActiveScene().name == "MainScene") {
            data.runActive = true;
            data.runData = new();
        }

        data.baseTerrain = Utils.CreateJaggedArray<string[][]>(Game.TerrainSize.x, Game.TerrainSize.y);
        for (int x = 0; x < Game.TerrainSize.x; x++) {
            for (int y = 0; y < Game.TerrainSize.y; y++) {
                data.baseTerrain[y][x] = Game.BaseTerrain[y][x].name;
            }
        }

        data.perkUnlockTracker = new(Game.perksUnlockTracker);
        data.placeableObjectUnlockTracker = new(Game.placeableObjectsUnlockTracker);

        data.factionData = (from faction in Game.BaseFactions select new FactionData(faction)).ToList();
        data.playerFaction = new(Game.PlayerFaction);

        data.baseObjectInfo = (from info in Game.baseObjectInfo select new BaseObjectInfoData(info)).ToList();

        data.resources = Game.GetResources();
        data.terrainSize = Game.TerrainSize;
    }

    public void LoadData(GameData data) {
        throw new NotImplementedException();
    }
}
