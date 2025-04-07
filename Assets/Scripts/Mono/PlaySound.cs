using UnityEngine;
using System;

public class PlaySound : MonoBehaviour {
    private AudioSource source;

    private bool startedPlaying;
    private bool createGameObject;
    private SOSound.SoundType soundType;
    public event EventHandler soundFinished;

    private float volume;

    public void SetSource(
        bool createGameObject,
        SOSound.SoundType soundType,
        AudioClip clip,
        float volume,
        float pitch,
        float spatial_blend
    ) {
        this.createGameObject = createGameObject;
        this.soundType = soundType;
        source.clip = clip;
        source.volume = this.volume = volume * GameManager.instance.GetVolumeScale(soundType);
        source.pitch = pitch;
        source.spatialBlend = spatial_blend;
    }

    public void Play() {
        source.Play();
        startedPlaying = true;
    }

    private void Update() {
        source.volume = volume * GameManager.instance.GetVolumeScale(soundType);
        if (startedPlaying && !source.isPlaying) {
            soundFinished?.Invoke(null, EventArgs.Empty);
            if (createGameObject) {
                Destroy(gameObject);
            } else {
                Destroy(source); Destroy(this);
            }
        }
    }

    private void Awake() {
        source = gameObject.AddComponent<AudioSource>();
    }
}
