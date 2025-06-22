using UnityEngine;

[CreateAssetMenu(fileName = "SODifficulty", menuName = "SODifficulty")]
public class SODifficulty : ScriptableObject {
    public string displayName;

    [Header("Effects")]
    public Mod[] modsToApply;
    
    [Header("Score Bonus")]
    public float mult;
}
