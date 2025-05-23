using UnityEngine;

[System.Serializable]
public class Vector2IntData {
    public int x;
    public int y;

    public Vector2IntData(Vector2Int vector2Int) {
        x = vector2Int.x;
        y = vector2Int.y;
    }

    public Vector2IntData() {}
    public Vector2Int Get() { return new(x, y); }
}
