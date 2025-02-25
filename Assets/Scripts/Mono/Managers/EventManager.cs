using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
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
            Debug.Log(currentEvent.GetDescription());
        }
    }

    private void Start() {
        positiveEvents = positiveEventsDict.GetDict();
        negativeEvents = negativeEventsDict.GetDict();
        Utils.GetManager<WaveManager>().waveEnd += WaveEnd;
    }
}
