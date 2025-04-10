using System;
using System.Collections.Generic;
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

    public void Event() {
        currentEvent.Event();
        currentEvent = null;
    }

    private void NewEvent() {
        Dictionary<SOEvent, int> potentialEvents = new();
        if (GameManager.Random.Next(1, 101) < negativeEventChance) {
            foreach (SOEvent event_ in negativeEvents.Keys) if (event_.IsValid()) potentialEvents.Add(event_, negativeEvents[event_]);
        } else {
            foreach (SOEvent event_ in positiveEvents.Keys) if (event_.IsValid()) potentialEvents.Add(event_, positiveEvents[event_]);
        }

        currentEvent = Utils.Choice(potentialEvents);
        currentEvent.Setup();
    }

    private void WaveEnd(object _, EventArgs __) {
        eventChance += eventChanceIncreasePerWave;
        if (GameManager.Random.Next(1, 101) < eventChance) {
            eventChance = 0f;
            NewEvent();
        }
    }

    private void Start() {
        instance = this;

        positiveEvents = positiveEventsDict.GetDict();
        negativeEvents = negativeEventsDict.GetDict();
        WaveManager.instance.waveEnd += WaveEnd;
    }

    public void SaveData(GameData data) {
        data.runData.eventChance = eventChance;
    }

    public void LoadData(GameData data) {
        eventChance = data.runData.eventChance;
    }
}
