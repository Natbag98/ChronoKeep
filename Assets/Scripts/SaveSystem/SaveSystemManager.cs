using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using GameDevWare.Serialization;
using UnityEngine.SceneManagement;

public class SaveSystemManager : MonoBehaviour {
    public static SaveSystemManager instance;

    [Header("Configuration")]
    [SerializeField] private string saveFileExtension;
    [SerializeField] private float saveFileVersion;
    [SerializeField] private string saveFolder;

    private string saveFolderPath;

    public void SaveGame(string save_name="Debug") {
        GameData data = new() { saveFileVersion = saveFileVersion };
        if (SceneManager.GetActiveScene().name == "MainScene") {
            data.runActive = true;
            data.runData = new();
        }

        List<ISaveSystem> saveSystemObjects = new(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISaveSystem>());
        foreach (ISaveSystem saveSystemObject in saveSystemObjects) saveSystemObject.SaveData(data);
        WriteToFile(saveFolderPath, save_name, Json.SerializeToString(data, SerializationOptions.PrettyPrint));
    }

    public void LoadGame(string save_name="Debug") {
        GameData data = Json.Deserialize<GameData>(LoadFromFile(saveFolderPath, save_name), SerializationOptions.PrettyPrint);
        List<ISaveSystem> saveSystemObjects = new(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<ISaveSystem>());
        foreach (ISaveSystem saveSystemObject in saveSystemObjects) saveSystemObject.LoadData(data);
    }

    public bool GetRunActive(string save_name="Debug") {
        GameData data = Json.Deserialize<GameData>(LoadFromFile(saveFolderPath, save_name), SerializationOptions.PrettyPrint);
        return data.runActive;
    }

    public bool GetCanLoadGame(string save_name="Debug") {
        if (!File.Exists(Path.Combine(saveFolderPath, $"{save_name}.{saveFileExtension}"))) return false;
        return Json.Deserialize<GameData>(LoadFromFile(saveFolderPath, save_name), SerializationOptions.PrettyPrint).saveFileVersion == saveFileVersion;
    }

    private void WriteToFile(string path, string filename, string data) {
        Directory.CreateDirectory(path);
        using FileStream stream = new(Path.Combine(path, $"{filename}.{saveFileExtension}"), FileMode.Create);
        using StreamWriter writer = new(stream);
        writer.Write(data);
    }

    private string LoadFromFile(string path, string filename) {
        using FileStream stream = new(Path.Combine(path, $"{filename}.{saveFileExtension}"), FileMode.Open);
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

        saveFolderPath = Path.Combine(Application.persistentDataPath, saveFolder);
        // Debug folder
        // saveFolderPath = Path.Combine(Application.dataPath, "TestSaves", saveFolder);
    }
}
