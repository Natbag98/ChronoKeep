using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Linq;

public class Utils : MonoBehaviour {
    public static T Choice<T>(T[] array) { return array[GameManager.Random.Next(array.Length)]; }
    public static T Choice<T>(List<T> list) { return list[GameManager.Random.Next(list.Count)]; }
    public static T Choice<T>(Dictionary<T, int> dict) {
        int max = 0;
        foreach (int chance in dict.Values) max += chance;
        int rand = GameManager.Random.Next(1, max);

        int threshold = 0;
        foreach (T key in dict.Keys) {
            threshold += dict[key];
            if (rand < threshold) {
                return key;
            }
        }

        return default;
    }

    /// <summary>
    /// Gets all the values in an enum as an enumerator
    /// </summary>
    public static IEnumerable<T> GetEnumValues<T>() {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// A dict wrapper that can be serialized by unity.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key.</typeparam>
    /// <typeparam name="TValue">The dictionary value.</typeparam>
    [System.Serializable]
    public class SerializeableDict<TKey, TValue> {
        [SerializeField] private SerializableDictPair[] dictPairs;

        [System.Serializable]
        private class SerializableDictPair {
            public TKey Key;
            public TValue Value;
        }

        public Dictionary<TKey, TValue> GetDict() {
            Dictionary<TKey, TValue> dict = new();
            foreach (SerializableDictPair pair in dictPairs) dict.Add(pair.Key, pair.Value);
            return dict;
        }
    }

    public class SerializableNullable<T> where T : class {
        [SerializeField] private bool containsValue;
        [SerializeField] private T value;
        public T GetValue() {
            if (containsValue) {
                return value;
            } else {
                return null;
            }
        }
    }

    public static T CreateJaggedArray<T>(params int[] lengths) { return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths); }
    public static bool SetFirstNull<T>(T element, T[] array) {
        for (int i = 0; i < array.Length; i++) if (array[i] == null) { array[i] = element; return true; }
        return false;
    }

    private static object InitializeJaggedArray(Type type, int index, int[] lengths) {
        Array array = Array.CreateInstance(type, lengths[index]);
        Type elementType = type.GetElementType();

        if (elementType != null) {
            for (int i = 0; i < lengths[index]; i++) {
                array.SetValue(InitializeJaggedArray(elementType, index + 1, lengths), i);
            }
        }

        return array;
    }

    #region Pathfinding

    private class PlotPathInfo {
        public int gCost;
        public int hCost;
        public int fCost;

        public Vector2Int? from_pos;

        public PlotPathInfo() {
            gCost = int.MaxValue;
            CalculateFCost();
            from_pos = null;
        }

        public void CalculateFCost() {
            fCost = gCost + hCost;
        }
    }

    private static int CalulateDistanceCost(Vector2Int a, Vector2Int b) {
        int x_distance = Mathf.Abs(a.x - b.x);
        int y_distance = Mathf.Abs(a.y - b.y);
        int diag = Mathf.Min(x_distance, y_distance);
        int non_diag = Mathf.Abs(x_distance - y_distance);
        return diag * 14 + non_diag * 10;
    }

    private static PlotPathInfo GetPlotPathInfo(PlotPathInfo[][] plot_path_info, Vector2Int pos) {
        return plot_path_info[pos.y][pos.x];
    }

    /// <summary>
    /// Gets a path of plots from start_pos to target_pos based an A* pathfinding algorithm.
    /// Note that the start plot is excluded from the final path.
    /// </summary>
    /// <param name="start_pos">The plotArray position of the start plot. (excl)</param>
    /// <param name="target_pos">The plotArray position of the end plot. (incl)</param>
    /// <returns></returns>
    public static List<Plot> GetPath(Vector2Int start_pos, Vector2Int target_pos) {
        PlotPathInfo[][] plot_path_info = CreateJaggedArray<PlotPathInfo[][]>(
            GameManager.instance.Game.TerrainSize.x,
            GameManager.instance.Game.TerrainSize.y
        );

        List<Vector2Int> plots_to_search = new(){ start_pos };
        List<Vector2Int> plots_searched = new();

        for (int x = 0; x < GameManager.instance.Game.TerrainSize.x; x++) {
            for (int y = 0; y < GameManager.instance.Game.TerrainSize.y; y++) {
                plot_path_info[y][x] = new();
            }
        }

        GetPlotPathInfo(plot_path_info, start_pos).gCost = 0;
        GetPlotPathInfo(plot_path_info, start_pos).hCost = CalulateDistanceCost(start_pos, target_pos);
        GetPlotPathInfo(plot_path_info, start_pos).CalculateFCost();

        while (plots_to_search.Count > 0) {
            Vector2Int current_pos = plots_to_search[0];
            for (int i = 1; i < plots_to_search.Count; i++) {
                if (GetPlotPathInfo(plot_path_info, plots_to_search[i]).fCost < GetPlotPathInfo(plot_path_info, current_pos).fCost) {
                    current_pos = plots_to_search[i];
                }
            }

            plots_to_search.Remove(current_pos);
            plots_searched.Add(current_pos);

            foreach (Plot neighbour in GetManager<RunManager>().GetPlotArray()[current_pos.y][current_pos.x].GetNeighbours()) {
                Vector2Int neighbour_pos = neighbour.GetPositionInPlotArray();
                PlotPathInfo neighbour_info = GetPlotPathInfo(plot_path_info, neighbour_pos);
                PlotPathInfo current_info = GetPlotPathInfo(plot_path_info, current_pos);

                if (neighbour_pos == target_pos) {
                    neighbour_info.from_pos = current_pos;
                    List<Plot> path = new();
                    Vector2Int current_path_pos = target_pos;
                    while (GetPlotPathInfo(plot_path_info, current_path_pos).from_pos != null) {
                        path.Add(GetManager<RunManager>().GetPlotArray()[current_path_pos.y][current_path_pos.x]);
                        current_path_pos = (Vector2Int)GetPlotPathInfo(plot_path_info, current_path_pos).from_pos;
                    }
                    path.Reverse();
                    return path;
                }

                if (plots_searched.Contains(neighbour_pos) || !neighbour.CanCharacterMoveThrough()) continue;

                int tentativeGCost = current_info.gCost + CalulateDistanceCost(current_pos, neighbour_pos);
                if (tentativeGCost < neighbour_info.gCost) {
                    neighbour_info.from_pos = current_pos;
                    neighbour_info.gCost = tentativeGCost;
                    neighbour_info.hCost = CalulateDistanceCost(neighbour_pos, target_pos);
                    neighbour_info.CalculateFCost();
                    if (!plots_to_search.Contains(neighbour_pos)) plots_to_search.Add(neighbour_pos);
                }
            }
        }

        return null;
    }

    #endregion

    public static void RotateTowards(Vector3 from_position, Vector3 to_position, Transform rotate, float rotate_speed=0f) {
        Vector3 direction = (to_position - from_position).normalized;
		Quaternion look_rotation = Quaternion.LookRotation(direction);
        if (rotate_speed == 0f) {
            rotate.rotation = look_rotation;
        } else {
            rotate.rotation = Quaternion.Slerp(rotate.rotation, look_rotation, Time.deltaTime * rotate_speed);
        }
    }

    public static float CalculateDamage(float amount, float defense) {
        return amount - defense;
    }

    public static T GetManager<T>() where T : UnityEngine.Object { return FindFirstObjectByType<T>(); }

    /// <summary>
    /// Checks whether the mouse is hovering over a UI element with the given tag.
    /// </summary>
    /// <param name="tag">The tag to check against.</param>
    /// <returns>The GameObject that the mouse is hovering over.</returns>
    public static GameObject CheckMouseHoveringOverUIElementWithTag(Tag.Tags tag) {
        PointerEventData pointerEventData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> raycastResult = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResult);
        foreach (RaycastResult result in raycastResult) {
            if (result.gameObject.GetComponent<Tag>() != null && result.gameObject.GetComponent<Tag>().HasTag(tag)) {
                return result.gameObject;
            }
        }
        return null;
    }

    
    public static List<T> GetAllAssets<T>() where T : UnityEngine.Object {
        List<T> to_return = new();
        foreach (string asset in AssetDatabase.FindAssets($"t:{typeof(T).Name}")) {
            to_return.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(asset)));
        }
        return to_return;
    }
}
