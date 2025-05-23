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
        ReloadSpeed,
        Attack,
        Range,
        CharacterMoveSpeed,
        ProjectileMoveSpeed,
        ExplosionRadius,
        Resistance,
        DamageReductionTower,
        DamageReductionCharacter,
        RangedMeleeAttackReduction
    }
    public enum PlaceableObjectTypes {
        Castle,
        Tower,
        Spawner,
        CivilianTower,
        Feature
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
        Gold,
        Mountains,
        Marsh
    }
    public enum PerkTrees {
        KingdomManagement,
        Engineering,
        Walls,
        Luck
    }
    public enum MagicTypes {
        Physical,
        Magic
    }

    [Header("Static Data : Values")]
    public float PlotMouseOverHeight;
    public float PlotMouseOverSpeed;

    public int MinBarbGenerationDistance;
    public int MaxBarbGenerationDistance;

    public float minGameSpeed;
    public float maxGameSpeed;

    public Utils.SerializeableDict<Attributes, int> defaultAttributes;

    [Header("Static Data : Scriptable Objects")]
    public SOPlaceableObject Castle;
    public SOPlaceableObject BarbCamp;
    public SOPlaceableObject ArcherTower;
    public SOPlot Plains;
    public SOFeature Ruins;
    public SOUpgrade WallsUpgrade;
    public SOPerk[] LuckPerks;

    [Header("Static Data : References")]
    public GameObject WallPrefab;

    [Header("Plot Generation Data")]
    [SerializeField] private int mapSize;
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> plotGenerationData;

    [Header("Test Data")]
    public bool debugMode;
    public Utils.SerializeableDict<Resources, int> startingResources;

    [HideInInspector] public Game Game;
    [HideInInspector] public TextData TextData = new();

    [HideInInspector] public string playerName;
    [HideInInspector] public string kingdomName;

    [HideInInspector] public List<SOPlaceableObject> allSOPlaceableObjects = new();

    [HideInInspector] public bool load = false;

    [HideInInspector] public int scoreLastRun;
    [HideInInspector] public int skillLastRun;

    [HideInInspector] public string nextScene;
    [HideInInspector] public int storyDisplay;

    [Header("SO References")]
    public List<ScriptableObject> scriptableObjects;

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        # if !UNITY_EDITOR
            Screen.SetResolution(1920, 1080, true);
        # endif

        foreach (SOPlaceableObject placeable_object in Utils.GetAllAssets<SOPlaceableObject>()) allSOPlaceableObjects.Add(placeable_object);

        if (SceneManager.GetActiveScene().name != "MainMenuScene") {
            Game = new(new(mapSize, mapSize), plotGenerationData.GetDict(), "", "");
        }
    }

    public void NewGame() {
        Game = new(new(mapSize, mapSize), plotGenerationData.GetDict(), kingdomName, playerName);
    }

    public float GetVolumeScale(SOSound.SoundType soundType) {
        return soundType switch {
            SOSound.SoundType.Music => (float)SettingsManager.instance.musicVolume / 100f,
            SOSound.SoundType.Effect => (float)SettingsManager.instance.soundEffectsVolume / 100f,
            _ => 1
        };
    }

    void Update() {
        if (load) {
            if (RunManager.instance != null || SceneManager.GetActiveScene().name == "GameScene") {
                load = false;
                SaveSystemManager.instance.LoadGame();
            }
        }

        // Game.DebugUpdate();
    }

    public void SaveData(GameData data) {
        data.baseTerrain = Utils.CreateJaggedArray<string[][]>(Game.TerrainSize.x, Game.TerrainSize.y);
        for (int x = 0; x < Game.TerrainSize.x; x++) {
            for (int y = 0; y < Game.TerrainSize.y; y++) {
                data.baseTerrain[y][x] = Game.BaseTerrain[y][x].name;
            }
        }

        data.perkUnlockTracker = new(Game.perksUnlockTracker);
        data.characterUnlockTracker = new(Game.characterUnlockTracker);
        data.placeableObjectUnlockTracker = new(Game.placeableObjectsUnlockTracker);

        data.factionData = (from faction in Game.BaseFactions select new FactionData(faction)).ToList();
        data.playerFaction = new(Game.PlayerFaction);

        data.baseObjectInfo = (from info in Game.baseObjectInfo select new BaseObjectInfoData(info)).ToList();

        data.resources = Game.GetResources();
        data.skill = Game.skillPoints;
        data.firstRun = Game.firstRun;
        data.terrainSize = new(Game.TerrainSize);
    }

    public void LoadData(GameData data) {
        Game.LoadData(data);
    }
}
