using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TextData {
    public Dictionary<string, List<string>> Data { private set; get; } = new();

    public TextData() {
        string data_path = Path.Combine(Application.dataPath, "TextData");
        DirectoryInfo info = new(data_path);
        FileInfo[] fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo) {
            Data.Add(info.Name, LoadListFromFile(info.FullName));
        }
    }

    static List<string> LoadListFromFile(string filePath) {
        List<string> list = new();
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines) {
            if (!string.IsNullOrWhiteSpace(line)) {
                list.Add(line.Trim());
            }
        }
        return list;
    }
}
