using UnityEngine;

public class RunManager : MonoBehaviour {
    public static RunManager instance;

    [Header("References")]
    [SerializeField] private Transform plotContainer;

    private Plot[][] plotArray;

    private void InstantiatePlots() {
        Game game = GameManager.instance.Game;
        plotArray = Utils.CreateJaggedArray<Plot[][]>(game.TerrainSize.x, game.TerrainSize.y);
        for (int x = 0; x < game.TerrainSize.x; x++) {
            for (int y = 0; y < game.TerrainSize.y; y++) {
                GameObject new_plot = Instantiate(
                    game.BaseTerrain[y][x].Prefab,
                    new Vector3(x - game.TerrainSize.x / 2, 0f, y - game.TerrainSize.y / 2),
                    Quaternion.identity,
                    plotContainer
                );
                plotArray[y][x] = new_plot.GetComponent<Plot>();
            }
        }

        plotArray[game.CastleLocation.y][game.CastleLocation.x].PlaceObject(GameManager.instance.Castle);
        foreach (Vector2Int location in game.BarbCamps) {
            plotArray[location.y][location.x].PlaceObject(GameManager.instance.BarbCamp);
        }
    }

    private void Start() {
        instance = this;
        InstantiatePlots();
    }
}
