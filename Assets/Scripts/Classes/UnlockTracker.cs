using System.Collections.Generic;

public class UnlockTracker<T> where T : UnityEngine.Object {
    public Dictionary<T, bool> disovered;
    public Dictionary<T, bool> unlocked;

    public UnlockTracker() {
        foreach (T asset in Utils.GetAllAssets<T>()) disovered.Add(asset, false);
    }

    public void UpdateDiscovered(T asset) {
        disovered[asset] = true;
    }

    public void UppdateUnlocked(T asset) {
        unlocked[asset] = true;
    }
}
