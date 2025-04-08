using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SOSound", menuName = "SOSound", order = 0)]
public class SOSound : ScriptableObject {
    [Header("Sound Attributes")]
    [SerializeField] private AudioClip clip;

    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    [SerializeField] [Range(.1f, 3f)] private float pitch = 1f;

    [Header("Sound Player Creation Data")]
    [SerializeField] private bool onCamera;
    [SerializeField] private bool createGameObject;

    [Header("Sound Type Data")]
    [SerializeField] private SoundType soundType;

    public enum SoundType {
        Music,
        Effect
    }

    public PlaySound Play(GameObject sender) {
        PlaySound play_sound;
        float spatial_blend;
        if (onCamera) {
            play_sound = Camera.allCameras[0].gameObject.AddComponent<PlaySound>();
            spatial_blend = 0;
        } else {
            if (createGameObject) {
                GameObject new_sound_object = new("Sound Player");
                play_sound = new_sound_object.AddComponent<PlaySound>();
            } else {
                play_sound = sender.AddComponent<PlaySound>();
            }
            spatial_blend = 1;
        }

        play_sound.SetSource(
            createGameObject,
            soundType,
            clip,
            volume,
            pitch,
            spatial_blend
        );

        play_sound.Play();
        return play_sound;
    }
}
