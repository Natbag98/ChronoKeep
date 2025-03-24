using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
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
}
