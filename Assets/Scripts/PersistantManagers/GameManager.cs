using System;
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
        Spawner
    }
    public enum FactionTypes {
        BarbarianClan,
        Kingdom
    }
    public enum Resources {
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

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "MainScene") {
            Game = new(new(11, 11), plotGenerationData.GetDict(), "", "");
        }
    }

    public void NewGame() {
        Game = new(new(11, 11), plotGenerationData.GetDict(), kingdomName, playerName);
    }
}
