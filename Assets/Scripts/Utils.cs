using UnityEngine;
using System;
using System.Collections.Generic;

public class Utils : MonoBehaviour {
    public static T Choice<T>(T[] array) { return array[GameManager.Random.Next(array.Length)]; }
    public static T Choice<T>(List<T> list) { return list[GameManager.Random.Next(list.Count)]; }

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

            foreach (Plot neighbour in RunManager.instance.GetPlotArray()[current_pos.y][current_pos.x].GetNeighbours()) {
                Vector2Int neighbour_pos = neighbour.GetPositionInPlotArray();
                PlotPathInfo neighbour_info = GetPlotPathInfo(plot_path_info, neighbour_pos);
                PlotPathInfo current_info = GetPlotPathInfo(plot_path_info, current_pos);

                if (neighbour_pos == target_pos) {
                    neighbour_info.from_pos = current_pos;
                    List<Plot> path = new();
                    Vector2Int current_path_pos = target_pos;
                    while (GetPlotPathInfo(plot_path_info, current_path_pos).from_pos != null) {
                        path.Add(RunManager.instance.GetPlotArray()[current_path_pos.y][current_path_pos.x]);
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
}
