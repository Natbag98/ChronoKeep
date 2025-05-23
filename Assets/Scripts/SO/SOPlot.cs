using UnityEngine;

[CreateAssetMenu(fileName = "SOPlot", menuName = "SOPlot")]
public class SOPlot : ScriptableObject {
    public string displayName;
    public string description;
    public GameObject prefab;
    public GameManager.PlotTypes plotType;
}
