using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class TextData {
    public Dictionary<string, List<string>> Data { private set; get; } = new();

    public TextData() {
        string data_path = Path.Combine(Application.dataPath, "TextData", "");
        DirectoryInfo info = new(data_path);
        FileInfo[] fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo) {
            string[] names = file.Name.Split(".");
            foreach (string name in names) if (name == "meta") continue;
            if (!Data.Keys.ToArray().Contains(names[0])) Data.Add(names[0], LoadListFromFile(Path.Combine(data_path, file.Name)));
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
