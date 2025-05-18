using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnlockTrackerData<T> where T : ScriptableObject, IUnlockTrackable {
    public Dictionary<string, bool> disovered = new();
    public Dictionary<string, bool> unlocked = new();

    public UnlockTrackerData(UnlockTracker<T> unlock_tracker) {
        foreach (string t in unlock_tracker.disovered.Keys) {
            disovered.Add(t, unlock_tracker.disovered[t]);
        }

        foreach (string t in unlock_tracker.unlocked.Keys) {
            unlocked.Add(t, unlock_tracker.unlocked[t]);
        }
    }

    public UnlockTrackerData() {}
}
