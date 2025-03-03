using UnityEngine;

[CreateAssetMenu(fileName = "SOPlot", menuName = "SOPlot")]
public class SOPlot : ScriptableObject {
    public string displayName;
    public GameObject prefab;
    public GameManager.PlotTypes plotType;
}
