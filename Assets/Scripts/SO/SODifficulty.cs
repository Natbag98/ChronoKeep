using UnityEngine;

[CreateAssetMenu(fileName = "SODifficulty", menuName = "SODifficulty")]
public class SODifficulty : ScriptableObject {
    public string displayName;

    [Header("Effects")]
    public int base_peace_cost;
    public int base_envoy_cost;
    public float aggroMult;
    public float powerMult;
    public Mod[] modsToApply;

    [Header("Starting Recources")]
    public SOPlaceableObject[] startingObjects;
    public Utils.SerializeableDict<GameManager.Resources, int> startingResources;
    
    [Header("Score Bonus")]
    public float mult;
}
