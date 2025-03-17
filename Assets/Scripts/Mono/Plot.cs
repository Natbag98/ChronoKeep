using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plot : MonoBehaviour {
    public static int neighbourUp = 0;
    public static int neighbourRight = 1;
    public static int neighbourDown = 2;
    public static int neighbourLeft = 3;

    [SerializeField] private bool canPlaceObject;

    [HideInInspector] public SOPlot plotSO;
    [HideInInspector] public GameManager.PlaceableObjectTypes? placedObjectType = null;
    [HideInInspector] public SOPlaceableObject placedObjectSO;
    [HideInInspector] public Faction faction;
    [HideInInspector] public GameManager.PlotTypes plotType;
    private Plot[] neighbours;
    private bool mouseOver;

    public bool GetCanPlaceObject() { return canPlaceObject; }

    /// <summary>
    /// Gets a list of neighbours based on steps.
    /// Will be in a circular shape if square is set to false.
    /// </summary>
    /// <param name="steps">The number of steps performed to find neighbours. Essentially the radius of the circle.</param>
    /// <param name="square">Will return a square of plots of steps size instead of a circle.</param>
    /// <returns>The list of neighbours.</returns>
    public List<Plot> GetNeighbours(int steps=1, bool square=false) {
        List<Plot> neighbours_to_return = new();
        List<Plot> neighbours_to_check = new() { this };

        int? max_range = null;
        if (square) {
            max_range = steps;
            steps *= 2;
        }

        for (int i = 0; i < steps; i++) {
            Plot[] temp_neighbours_to_check = new Plot[neighbours_to_check.Count];
            neighbours_to_check.CopyTo(temp_neighbours_to_check);

            foreach (Plot neighbour_to_check in temp_neighbours_to_check) {
                neighbours_to_check.Remove(neighbour_to_check);
                foreach (Plot neighbour in neighbour_to_check.neighbours) {
                    if (neighbour != null) {
                        if (max_range != null) {
                            if (
                                Mathf.Abs(GetPositionInPlotArray().x - neighbour.GetPositionInPlotArray().x) > max_range ||
                                Mathf.Abs(GetPositionInPlotArray().y - neighbour.GetPositionInPlotArray().y) > max_range
                            ) {
                                continue;
                            }
                        }
                        if (!neighbours_to_return.Contains(neighbour)) neighbours_to_return.Add(neighbour);
                        neighbours_to_check.Add(neighbour);
                    }
                }
            }
        }
        return neighbours_to_return;
    }

    /// <summary>
    /// Gets a list of characters standing on this plot.
    /// </summary>
    public List<Character> GetCharacters() {
        List<Character> characters = new();
        foreach (Character character in FindObjectsByType<Character>(FindObjectsSortMode.None)) {
            if (character.GetCurrentPlot() == this) characters.Add(character);
        }
        return characters;
    }

    public void SetNeighbours(Plot[] neighbours) { this.neighbours = neighbours; }

    /// <summary>
    /// Checks whether a character can move through the plot.
    /// </summary>
    public bool CanCharacterMoveThrough() {
        if (placedObjectType != null) {
            return false;
        } else {
            return canPlaceObject;
        }
    }

    /// <summary>
    /// Places an object of the plot. Also sets the neighbouring plots faction ownership based on the objects factionControlRange.
    /// </summary>
    /// <param name="object_to_place">The object to place.</param>
    /// <param name="faction">The faction to set neighbouring plots onwership to.</param>
    public void PlaceObject(SOPlaceableObject object_to_place, Faction faction, bool player_placement=false) {
        if (player_placement && !GameManager.instance.Game.SpendResources(object_to_place.placementCost.GetDict())) return;
        Utils.GetManager<MainSceneUIManager>().ObjectPlaced();
        PlaceableObject new_object = Instantiate(
            object_to_place.placeableObjectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        ).GetComponent<PlaceableObject>();
        placedObjectType = new_object.objectType = object_to_place.objectType;
        placedObjectSO = new_object.placeableObjectSO = object_to_place;
        new_object.parentPlot = this;

        this.faction = faction;
        foreach (Plot plot in GetNeighbours(object_to_place.factionControlRange, true)) plot.faction ??= faction;
    }

    /// <summary>
    /// Gets the plots (x, y) position in the plotArray, which will be different from the plots position in the scene.
    /// </summary>
    /// <returns>The plots (x, y) positon in the plotArray</returns>
    public Vector2Int GetPositionInPlotArray() {
        return new(
            (int)transform.position.x + GameManager.instance.Game.TerrainSize.x / 2,
            (int)transform.position.z + GameManager.instance.Game.TerrainSize.y / 2
        );
    }

    /// <summary>
    /// Checks whether the an object can be placed on the plot.
    /// </summary>
    private bool ValidTowerPlacement(SOPlaceableObject object_to_place) {
        foreach (GameManager.PlotTypes plot_type in object_to_place.mustPlaceBeside) {
            if (!(from plot in GetNeighbours() select plot.plotType).Contains(plot_type)) return false;
        }
        if (
            placedObjectType != null ||
            faction != GameManager.instance.Game.PlayerFaction
        ) {
            return false;
        } else {
            return canPlaceObject;
        }
    }

    public void OnMouseEnter() {
        mouseOver = true;
        if (Utils.GetManager<MainSceneUIManager>().IsPlacingObject()) {
            if (ValidTowerPlacement(Utils.GetManager<MainSceneUIManager>().GetObjectToPlace())) {
                Utils.GetManager<MainSceneUIManager>().plotInfoPanel.SetActive(true);
            }
        } else if (!Utils.GetManager<MainSceneUIManager>().mouseBlocked) {
            Utils.GetManager<MainSceneUIManager>().plotInfoPanel.SetActive(true);
        }
        if (placedObjectType != null) Utils.GetManager<MainSceneUIManager>().objectInfoPanel.SetActive(true);
    }

    public void OnMouseExit() {
        mouseOver = false;
        Utils.GetManager<MainSceneUIManager>().plotInfoPanel.SetActive(false);
        Utils.GetManager<MainSceneUIManager>().objectInfoPanel.SetActive(false);
    }

    public void OnMouseDown() {
        if (Utils.GetManager<MainSceneUIManager>().IsPlacingObject() && Utils.GetManager<MainSceneUIManager>().GetObjectToPlace()) {
            PlaceObject(Utils.GetManager<MainSceneUIManager>().GetObjectToPlace(), GameManager.instance.Game.PlayerFaction, true);
        }
    }

    private void Update() {
        float target_height;
        if (
            mouseOver &&
            !Utils.GetManager<WaveManager>().waveActive
        ) {
            if (
                Utils.GetManager<MainSceneUIManager>().mouseBlocked ||
                Utils.GetManager<MainSceneUIManager>().IsPlacingObject() && 
                !ValidTowerPlacement(Utils.GetManager<MainSceneUIManager>().GetObjectToPlace())
            ) {
                target_height = 0;
            } else {
               target_height = GameManager.instance.PlotMouseOverHeight;

                Utils.GetManager<MainSceneUIManager>().plotInfoName.text = plotSO.displayName;
                if (placedObjectType != null) {
                    Utils.GetManager<MainSceneUIManager>().objectInfoName.text = placedObjectSO.displayName;
                }
            }
        } else {
            target_height = 0;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, target_height, transform.position.z),
            GameManager.instance.PlotMouseOverSpeed * Time.deltaTime
        );
    }
}
