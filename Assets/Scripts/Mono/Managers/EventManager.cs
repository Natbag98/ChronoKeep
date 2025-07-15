using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour, ISaveSystem {
    public static EventManager instance;

    [Header("Events")]
    [SerializeField] private Utils.SerializeableDict<SOEvent, int> positiveEventsDict;
    [SerializeField] private Utils.SerializeableDict<SOEvent, int> negativeEventsDict;

    [Header("Attributes")]
    [SerializeField] private int eventChanceIncreasePerWave;
    [SerializeField] private int negativeEventChance;

    [HideInInspector] public SOEvent currentEvent;

    private Dictionary<SOEvent, int> positiveEvents;
    private Dictionary<SOEvent, int> negativeEvents;

    private float eventChance;
    private List<SOEvent> eventList = new();
    
    private int shopChance = 0;

    public void Event() {
        currentEvent.Event();
        currentEvent = null;
    }

    private void NewEvent() {
        int negative_event_chance = negativeEventChance;
        if (GameManager.instance.Game.perksUnlockTracker.unlocked[GameManager.instance.LuckPerks[0]]) negative_event_chance -= 5;
        if (GameManager.instance.Game.perksUnlockTracker.unlocked[GameManager.instance.LuckPerks[1]]) negative_event_chance -= 5;
        if (GameManager.instance.Game.perksUnlockTracker.unlocked[GameManager.instance.LuckPerks[2]]) negative_event_chance -= 10;

        Dictionary<SOEvent, int> potentialEvents = new();
        if (GameManager.Random.Next(1, 101) < negative_event_chance) {
            foreach (SOEvent event_ in negativeEvents.Keys) if (event_.IsValid()) potentialEvents.Add(event_, negativeEvents[event_]);
        } else {
            foreach (SOEvent event_ in positiveEvents.Keys) if (event_.IsValid()) potentialEvents.Add(event_, positiveEvents[event_]);
        }

        SOEvent new_event = Utils.Choice(potentialEvents);
        eventList.Add(new_event);
    }

    private void WaveEnd(object _, EventArgs __) {
        eventList = new();
        eventChance += eventChanceIncreasePerWave;
        if (GameManager.Random.Next(1, 101) < eventChance) {
            eventChance = 0f;
            NewEvent();
        }

        if (
            (
                from plot
                in RunManager.instance.GetAllVisiblePlots()
                where plot.placedObjectType == GameManager.PlaceableObjectTypes.Spawner && plot.faction == GameManager.instance.Game.BaseFactions[^1]
                select plot
            ).ToList().Count == 0
        ) {
            eventList.Add(Utils.GetAsset<PlaceObject>("PlaceVisibleBarbCamp"));
        }

        if (GameManager.Random.Next(100) < shopChance) {
            MainSceneUIManager.instance.shopActive = true;
            shopChance = 0;
        } else {
            shopChance += 10;
        }
    }

    private void Start() {
        instance = this;

        positiveEvents = positiveEventsDict.GetDict();
        negativeEvents = negativeEventsDict.GetDict();
        WaveManager.instance.waveEnd += WaveEnd;
    }

    void Update() {
        if (eventList.Count > 0 && currentEvent == null) {
            currentEvent = eventList[0];
            eventList.RemoveAt(0);
            currentEvent.Setup();
        }
    }

    public void SaveData(GameData data) {
        data.runData.eventChance = eventChance;
    }

    public void LoadData(GameData data) {
        eventChance = data.runData.eventChance;
    }
}
