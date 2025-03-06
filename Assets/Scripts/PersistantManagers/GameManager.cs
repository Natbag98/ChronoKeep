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
        Gold
    }
    public enum PlotTypes {
        Plains,
        Lake,
        Gold
    }

    [Header("Static Data")]
    public float PlotMouseOverHeight;
    public float PlotMouseOverSpeed;

    public int MinBarbGenerationDistance;

    public SOPlaceableObject Castle;
    public SOPlaceableObject BarbCamp;

    [Header("Plot Generation Data")]
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> plotGenerationData;

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

        if (SceneManager.GetActiveScene().name == "MainScene") {
            Game = new(new(11, 11), plotGenerationData.GetDict(), "", "");
        }
    }

    public void NewGame() {
        Game = new(new(11, 11), plotGenerationData.GetDict(), kingdomName, playerName);
    }

    void Update() {
        Game.DebugUpdate();
    }
}
