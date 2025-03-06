using System.Collections.Generic;

public class UnlockTracker<T> where T : UnityEngine.Object, IUnlockTrackable {
    public Dictionary<T, bool> disovered;
    public Dictionary<T, bool> unlocked;

    public UnlockTracker() {
        foreach (T asset in Utils.GetAllAssets<T>()) disovered.Add(asset, false);
    }

    public void UpdateDiscovered(T asset) {
        disovered[asset] = true;
    }

    public void UpdateUnlocked(T asset) {
        unlocked[asset] = true;
    }

    public T GetRandomUnlocked(Tag.Tags[] tags) {
        List<T> potential = new();
        foreach (T asset in unlocked.Keys) {
            if (unlocked[asset]) {
                foreach (Tag.Tags tag in tags) {
                    if ((bool)asset.GetPrefab().GetComponent<Tag>()?.HasTag(tag)) {
                        potential.Add(asset);
                        continue;
                    }
                }
            }
        }
        return Utils.Choice(potential);
    }
}
