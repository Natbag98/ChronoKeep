using UnityEngine;

public interface ISaveSystem {
    public void SaveData(GameData data);
    public void LoadData(GameData data);
}
