using System.Collections.Generic;
using UnityEngine;

public class UnlockTracker<T> where T : ScriptableObject, IUnlockTrackable {
    public Dictionary<string, bool> disovered = new();
    public Dictionary<string, bool> unlocked = new();

    public UnlockTracker() {
        foreach (T asset in Utils.GetAllAssets<T>()) { disovered.Add(asset.name, false); unlocked.Add(asset.name, false); }
    }

    public void UpdateDiscovered(T asset) {
        disovered[asset.name] = true;
    }

    public void UpdateUnlocked(T asset) {
        unlocked[asset.name] = true;
    }

    public List<T> GetAllUnlocked() {
        List<T> unlocked = new();
        foreach (string t in this.unlocked.Keys) if (this.unlocked[t]) unlocked.Add(Utils.GetAsset<T>(t));
        return unlocked;
    }

    public T GetRandomUnlocked(Tag.Tags[] tags) {
        List<T> potential = new();
        foreach (string asset in unlocked.Keys) {
            if (unlocked[asset]) {
                foreach (Tag.Tags tag in tags) {
                    Tag tag_component = Utils.GetAsset<T>(asset).GetPrefab().GetComponent<Tag>();
                    if (tag_component != null && tag_component.HasTag(tag)) {
                        potential.Add(Utils.GetAsset<T>(asset));
                        continue;
                    }
                }
            }
        }
        return Utils.Choice(potential);
    }
}
