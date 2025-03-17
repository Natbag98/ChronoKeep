using System.Collections.Generic;
using UnityEngine;

public class UnlockTracker<T> where T : ScriptableObject, IUnlockTrackable {
    public Dictionary<T, bool> disovered = new();
    public Dictionary<T, bool> unlocked = new();

    public UnlockTracker() {
        foreach (T asset in Utils.GetAllAssets<T>()) { disovered.Add(asset, false); unlocked.Add(asset, false); }
    }

    public void UpdateDiscovered(T asset) {
        disovered[asset] = true;
    }

    public void UpdateUnlocked(T asset) {
        unlocked[asset] = true;
    }

    public List<T> GetAllUnlocked() {
        List<T> unlocked = new();
        foreach (T t in this.unlocked.Keys) if (this.unlocked[t]) unlocked.Add(t);
        return unlocked;
    }

    public T GetRandomUnlocked(Tag.Tags[] tags) {
        List<T> potential = new();
        foreach (T asset in unlocked.Keys) {
            if (unlocked[asset]) {
                foreach (Tag.Tags tag in tags) {
                    Tag tag_component = asset.GetPrefab().GetComponent<Tag>();
                    if (tag_component != null && tag_component.HasTag(tag)) {
                        potential.Add(asset);
                        continue;
                    }
                }
            }
        }
        return Utils.Choice(potential);
    }
}
