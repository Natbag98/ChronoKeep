using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour {
    [MenuItem("Assets/Update GameManager SO Assets")]
    static void UpdateGameManagerSOAssets() {
        GameObject game_manager = null;
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(GameObject).Name}")) {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset)).name == "GameManager") {
                game_manager = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(asset));
            }
        }

        string scriptable_object_search_folder = Path.Join(Application.dataPath, "ScriptableObjects");
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(ScriptableObject).Name}", new[] { scriptable_object_search_folder })) {
            Debug.Log(AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(asset)));
        }
    }
}
