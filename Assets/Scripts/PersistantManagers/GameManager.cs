using System;
using UnityEngine;

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
        ProjectileMoveSpeed
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

    [Header("Static Data")]
    public float PlotMouseOverHeight;
    public float PlotMouseOverSpeed;

    public int MinBarbGenerationDistance;

    public SOPlaceableObject Castle;
    public SOPlaceableObject BarbCamp;

    [Header("Plot Generation Data")]
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> PlotGenerationData;

    [HideInInspector] public Game Game;
    [HideInInspector] public TextData TextData = new();

    [HideInInspector] public string playerName;
    [HideInInspector] public string kingdomName;

    private bool test = true;
    [SerializeField] private SOPlaceableObject testPlacement;

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void NewGame() {
        Game = new(new(11, 11), PlotGenerationData.GetDict(), kingdomName, playerName);
    }

    void Update() {
        if (test) {
            test = false;
            MainSceneUIManager.instance.PlaceInventoryItem(testPlacement);
        }

        Debug.Log(Game.PlayerFaction.Ruler);
        Debug.Log(Game.PlayerFaction.Name);
    }
}
