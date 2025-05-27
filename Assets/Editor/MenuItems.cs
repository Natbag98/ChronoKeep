using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour {
    [MenuItem("Assets/Update GameManager SO Assets")]
    static void UpdateGameManagerSOAssets() {
        GameManager game_manager = null;
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(GameObject).Name}", new[] { Path.Join("Assets", "Prefabs", "Managers") })) {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset)).name == "GameManager") {
                game_manager = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset)).GetComponent<GameManager>();
            }
        }
        
        game_manager.scriptableObjects.Clear();
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(ScriptableObject).Name}", new[] { Path.Join("Assets", "ScriptableObjects") })) {
            game_manager.scriptableObjects.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(asset)));
        }

        Debug.Log($"Updated {game_manager.scriptableObjects.Count} SO assets");
    }
}
