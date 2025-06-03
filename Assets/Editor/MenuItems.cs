using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour {
    [MenuItem("Assets/Update GameManager SO Assets")]
    static void UpdateGameManagerSOAssets() {
        GameObject game_manager = null;
        string path = "";
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(GameObject).Name}", new[] { Path.Join("Assets", "Prefabs", "Managers") })) {
            path = AssetDatabase.GUIDToAssetPath(asset);
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path).name == "GameManager") game_manager = PrefabUtility.LoadPrefabContents(path);
            break;
        }

        game_manager.GetComponent<GameManager>().scriptableObjects.Clear();
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(ScriptableObject).Name}", new[] { Path.Join("Assets", "ScriptableObjects") })) {
            game_manager.GetComponent<GameManager>().scriptableObjects.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(asset)));
        }

        PrefabUtility.SaveAsPrefabAsset(game_manager, path);
        Debug.Log($"Updated {game_manager.GetComponent<GameManager>().scriptableObjects.Count} SO assets");
        PrefabUtility.UnloadPrefabContents(game_manager);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
