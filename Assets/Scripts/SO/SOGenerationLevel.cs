using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SOGenerationLevel", menuName = "SOGenerationLevel")]
public class SOGenerationLevel : ScriptableObject {
    public bool baseLevel;
    public SOPlot plot;
    public Utils.SerializeableDict<SOGenerationLevel, float> levels;

    public int GetMaxDepth(int depth) {
        depth++;
        if (baseLevel) {
            return depth;
        } else {
            int max_depth = 0;
            foreach (SOGenerationLevel level in levels.GetDict().Keys) if (level.GetMaxDepth(depth) > max_depth) max_depth = level.GetMaxDepth(depth);
            return max_depth;
        }
    }

    public void Check() {
        if (baseLevel) {
            if (plot == null) throw new Exception($"SOGenerationLevel {name} is missing a plot");
        } else {
            if (levels.GetDict().Count == 0) throw new Exception($"SOGenerationLevel {name} is missing a set of levels");
        }
    }
}
