using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using GameDevWare.Serialization;

public class SaveSystemManager : MonoBehaviour {
    public static SaveSystemManager instance;

    [Header("Configuration")]
    [SerializeField] private string saveFileExtension;
    [SerializeField] private string saveFolder;
    [SerializeField] private string playerDataFileName;

    [Header("Data")]
    [SerializeField] private GameObject[] plotPrefabs;

    private string saveFolderPath;

    public GameObject GetPlotPrefab(string prefab_name) {
        foreach (GameObject plot in plotPrefabs) if (plot.name == prefab_name) return plot;
        return null;
    }

    public void SaveGame(string save_name="Debug") {
        GameData data = new();
        List<ISaveSystem> saveSystemObjects = new(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveSystem>());
        foreach (ISaveSystem saveSystemObject in saveSystemObjects) saveSystemObject.SaveData(data);
        WriteToFile(saveFolderPath, save_name, Json.SerializeToString(data, SerializationOptions.PrettyPrint));
    }

    public void LoadGame(string save_name="Debug") {
        GameData data = Json.Deserialize<GameData>(LoadFromFile(saveFolderPath, save_name), SerializationOptions.PrettyPrint);
        List<ISaveSystem> saveSystemObjects = new(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveSystem>());
        foreach (ISaveSystem saveSystemObject in saveSystemObjects) saveSystemObject.LoadData(data);
    }

    private void WriteToFile(string path, string filename, string data) {
        Directory.CreateDirectory(path);
        using FileStream stream = new(System.IO.Path.Combine(path, $"{filename}.{saveFileExtension}"), FileMode.Create);
        using StreamWriter writer = new(stream);
        writer.Write(data);
    }

    private string LoadFromFile(string path, string filename) {
        using FileStream stream = new(System.IO.Path.Combine(path, $"{filename}.{saveFileExtension}"), FileMode.Open);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    private void Start() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        saveFolderPath = System.IO.Path.Combine(Application.persistentDataPath, saveFolder);
    }
}
