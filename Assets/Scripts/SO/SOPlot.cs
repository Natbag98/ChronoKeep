using UnityEngine;

[CreateAssetMenu(fileName = "SOPlot", menuName = "SOPlot")]
public class SOPlot : ScriptableObject {
    public GameObject Prefab;
    public GameManager.PlotTypes plotType;
}
