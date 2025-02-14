using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public static System.Random Random = new();

    public enum Attributes {
        Health,
        Defense
    }

    [Header("Static Data")]
    public float PlotMouseOverHeight;
    public float PlotMouseOverSpeed;

    [Header("Plot Generation Data")]
    [SerializeField] private Utils.SerializeableDict<SOPlot, int> PlotGenerationData;

    [HideInInspector] public Game Game;

    private void Awake() {
        if (instance) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Game = new(new(10, 10), PlotGenerationData.GetDict());
    }
}
