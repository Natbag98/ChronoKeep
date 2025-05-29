using UnityEngine;

[System.Serializable]
public class ReplacePlot {
    public SOPlot plot;
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> replaceChance;

    public bool Replace(SOPlot replace_plot) {
        if (replaceChance.GetDict().ContainsKey(replace_plot)) {
            if (replaceChance.GetDict()[replace_plot] > GameManager.Random.Next(100)) return true;
        }
        return false;
    }
}
