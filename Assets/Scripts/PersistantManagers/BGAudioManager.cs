using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGAudioManager : MonoBehaviour {
    public static BGAudioManager instance;

    [SerializeField] private SOSound mainMenuSound;
    [SerializeField] private List<SOSound> mainGameSounds;

    private SOSound lastSound;

    private void SoundFinished(object _, EventArgs __) {
        NewBGSound();
    }

    public void NewBGSound() {
        SOSound sound;
        if (SceneManager.GetActiveScene().name == "MainScene") {
            sound = Utils.Choice(mainGameSounds);
            if (lastSound != null) mainGameSounds.Add(lastSound);
            mainGameSounds.Remove(sound);
            lastSound = sound;
        } else {
            sound = mainMenuSound;
        }

        PlaySound play_sound = sound.Play(null);
        play_sound.soundFinished += SoundFinished;
    }

    private void ActiveSceneChanged(Scene _, Scene __) {
        NewBGSound();
    }

    private void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        NewBGSound();
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }
}
