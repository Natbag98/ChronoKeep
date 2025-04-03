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
        if (GameManager.Random.Next(1, 101) < negativeEventChance) {
            currentEvent = Utils.Choice(negativeEvents);
        } else {
            currentEvent = Utils.Choice(positiveEvents);
        }
        if (currentEvent.IsValid()) {
            currentEvent.Setup();
        } else {
            Debug.Log("Event was not valid"); // TODO : Replace with getting a new event once more events exist
            currentEvent = null;
        }
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
