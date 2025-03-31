using System.Collections.Generic;
using UnityEngine;

public class UnlockTrackerData<T> where T : ScriptableObject, IUnlockTrackable {
    public Dictionary<string, bool> disovered = new();
    public Dictionary<string, bool> unlocked = new();

    public UnlockTrackerData(UnlockTracker<T> unlock_tracker) {
        foreach (T t in unlock_tracker.disovered.Keys) {
            disovered.Add(t.name, unlock_tracker.disovered[t]);
        }

        foreach (T t in unlock_tracker.unlocked.Keys) {
            unlocked.Add(t.name, unlock_tracker.unlocked[t]);
        }
    }

    public UnlockTrackerData() {}
}
