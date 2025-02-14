using UnityEngine;

public class RunManager : MonoBehaviour {
    public static RunManager instance;

    [Header("References")]
    [SerializeField] private Transform plotContainer;

    private Plot[][] plotArray;

    private void InstantiatePlots() {
        plotArray = Utils.CreateJaggedArray<Plot[][]>(GameManager.instance.Game.TerrainSize.x, GameManager.instance.Game.TerrainSize.y);
        for (int x = 0; x < GameManager.instance.Game.TerrainSize.x; x++) {
            for (int y = 0; y < GameManager.instance.Game.TerrainSize.y; y++) {
                GameObject new_plot = Instantiate(
                    GameManager.instance.Game.BaseTerrain[y][x].Prefab,
                    new Vector3(x - GameManager.instance.Game.TerrainSize.x / 2, 0f, y - GameManager.instance.Game.TerrainSize.y / 2),
                    Quaternion.identity,
                    plotContainer
                );
                plotArray[y][x] = new_plot.GetComponent<Plot>();
            }
        }
    }

    private void Start() {
        instance = this;
        InstantiatePlots();
    }
}
