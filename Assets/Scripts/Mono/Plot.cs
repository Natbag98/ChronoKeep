using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Plot : MonoBehaviour {
    public static int neighbourUp = 0;
    public static int neighbourRight = 1;
    public static int neighbourDown = 2;
    public static int neighbourLeft = 3;

    [Header("Attributes")]
    [SerializeField] private bool canPlaceObject;
    public bool walkable;
    public Mod[] modsToApply;
    [SerializeField] private float placementHeight;

    [HideInInspector] public SOPlot plotSO;
    [HideInInspector] public GameManager.PlaceableObjectTypes? placedObjectType = null;
    [HideInInspector] public SOPlaceableObject placedObjectSO;
    [HideInInspector] public SOFeature placedFeatureSO;
    [HideInInspector] public Faction faction;
    [HideInInspector] public GameManager.PlotTypes plotType;
    [HideInInspector] public bool rangeFinding;
    private Plot[] neighbours;
    private bool mouseOver;
    public bool visibleToPlayer { get; private set; } = false;

    public bool GetCanPlaceObject(SOPlaceableObject p_object=null) {
        if (p_object != null) {
            if (p_object.objectType == GameManager.PlaceableObjectTypes.Spawner || p_object.objectType == GameManager.PlaceableObjectTypes.Castle) {
                if (!walkable) return false;
            }
        }
        return canPlaceObject;
    }

    public void SetVisibleToPlayer(bool set) {
        visibleToPlayer = set;
        SetVisible(set);
    }

    /// <summary>
    /// Gets a list of neighbours based on steps.
    /// Will be in a circular shape if square is set to false.
    /// </summary>
    /// <param name="steps">The number of steps performed to find neighbours. Essentially the radius of the circle.</param>
    /// <param name="square">Will return a square of plots of steps size instead of a circle.</param>
    /// <returns>The list of neighbours.</returns>
    public List<Plot> GetNeighbours(int steps=1, bool square=false, bool include_self=true) {
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

        if (!include_self) neighbours_to_return.Remove(this);
        return neighbours_to_return;
    }

    /// <summary>
    /// Gets a list of characters standing on this plot.
    /// </summary>
    public List<Character> GetCharacters() {
        List<Character> characters = new();
        foreach (Transform t in RunManager.instance.characterContainer) {
            Character character = t.GetComponent<Character>();
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
            return walkable;
        }
    }

    /// <summary>
    /// Places an object of the plot. Also sets the neighbouring plots faction ownership based on the objects factionControlRange.
    /// </summary>
    /// <param name="object_to_place">The object to place.</param>
    /// <param name="faction">The faction to set neighbouring plots onwership to.</param>
    /// <returns>Returns the placed object</returns>
    public PlaceableObject PlaceObject(SOPlaceableObject object_to_place, Faction faction=null, bool player_placement=false) {
        if (player_placement && !GameManager.instance.Game.SpendResources(object_to_place.placementCost.GetDict())) return null;
        MainSceneUIManager.instance.ObjectPlaced();
        PlaceableObject new_object = Instantiate(
            object_to_place.placeableObjectPrefab,
            new Vector3(transform.position.x, transform.position.y + placementHeight, transform.position.z),
            Quaternion.identity,
            transform
        ).GetComponent<PlaceableObject>();
        placedObjectType = new_object.objectType = object_to_place.objectType;
        placedObjectSO = new_object.placeableObjectSO = object_to_place;
        new_object.parentPlot = this;

        if (faction != null) {
            this.faction = faction;
            foreach (Plot plot in GetNeighbours(object_to_place.factionControlRange, true)) {
                if (this.faction == GameManager.instance.Game.PlayerFaction) plot.SetVisibleToPlayer(true);
                plot.faction ??= faction;
            }
        }

        return new_object;
    }

    public void PlaceFeature(SOFeature feature_to_place) {
        Instantiate(
            feature_to_place.prefab,
            transform.position,
            Quaternion.identity,
            transform
        );
        placedObjectType = GameManager.PlaceableObjectTypes.Feature;
        placedFeatureSO = feature_to_place;
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
        if (object_to_place.objectType == GameManager.PlaceableObjectTypes.Spawner && !walkable) return false;
        foreach (GameManager.PlotTypes plot_type in object_to_place.mustPlaceBeside) {
            if (!(from plot in GetNeighbours() select plot.plotType).Contains(plot_type)) return false;
        }

        if (object_to_place.mustPlaceBesideTags != null) {
            foreach (Tag.Tags tag in object_to_place.mustPlaceBesideTags) {
                if (!(
                    from plot in GetNeighbours()
                    where plot.GetComponentInChildren<PlaceableObject>() != null && plot.GetComponentInChildren<PlaceableObject>().GetComponent<Tag>() != null
                    select plot.GetComponentInChildren<PlaceableObject>().GetComponent<Tag>().HasTag(tag)
                ).Contains(true)) return false;
            }
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

    private void SetVisible(bool set) {
        if (GameManager.instance.debugMode) return;
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = set;
        foreach (Canvas canvas in GetComponentsInChildren<Canvas>()) canvas.enabled = set;
    }

    public void OnMouseEnter() {
        mouseOver = true;
    }

    public void OnMouseExit() {
        mouseOver = false;
        if (MainSceneUIManager.instance.IsPlacingObject()) SetRangeFinding(false);
        MainSceneUIManager.instance.plotInfoPanel.SetActive(false);
        MainSceneUIManager.instance.objectInfoPanel.SetActive(false);
    }

    public void OnMouseDown() {
        if (
            MainSceneUIManager.instance.IsPlacingObject() &&
            MainSceneUIManager.instance.GetObjectToPlace() &&
            ValidTowerPlacement(MainSceneUIManager.instance.GetObjectToPlace())
        ) {
            PlaceObject(MainSceneUIManager.instance.GetObjectToPlace(), GameManager.instance.Game.PlayerFaction, true);
        } else if (faction == GameManager.instance.Game.PlayerFaction && placedObjectSO != null && !MainSceneUIManager.instance.mouseBlocked) {
            foreach (SOUpgrade upgrade in Utils.GetAllAssets<SOUpgrade>()) {
                if (upgrade.IsAvailable(GetComponentInChildren<PlaceableObject>())) MainSceneUIManager.instance.InitializeUpgradesMenu(this);
                return;
            }
        }
    }

    private void Start() {
        SetVisible(false);
    }

    private void SetRangeFinding(bool set) {
        foreach (
            Plot plot in GetNeighbours(
                MainSceneUIManager.instance.GetObjectToPlace().placeableObjectPrefab.GetComponent<PlaceableObject>().GetRange(this), include_self: false
            )
        ) {
            plot.rangeFinding = set;
        }
    }

    private void Update() {
        if (RunManager.instance.paused) {
            MainSceneUIManager.instance.plotInfoPanel.SetActive(false);
            MainSceneUIManager.instance.objectInfoPanel.SetActive(false);
            return;
        }
        if (!visibleToPlayer) mouseOver = false;
        float target_height = 0;
        if (!MainSceneUIManager.instance.mouseBlocked) {
            if (
                mouseOver &&
                !WaveManager.instance.waveActive
            ) {
                if (faction != null && GameManager.instance.debugMode) Debug.Log(faction.Name);
                if (MainSceneUIManager.instance.IsPlacingObject()) {
                    if (ValidTowerPlacement(MainSceneUIManager.instance.GetObjectToPlace())) {
                        target_height = GameManager.instance.PlotMouseOverHeight;
                        SetRangeFinding(true);
                    }
                } else {
                    target_height = GameManager.instance.PlotMouseOverHeight;
                }
            }
        }

        if (!MainSceneUIManager.instance.IsPlacingObject()) {
            rangeFinding = false;
        }

        if (mouseOver) {
            MainSceneUIManager.instance.plotInfoName.text = plotSO.displayName;
            MainSceneUIManager.instance.plotInfoDescription.text = plotSO.description;
            if (placedObjectType != null) {
                if (placedObjectType == GameManager.PlaceableObjectTypes.Feature) {
                    MainSceneUIManager.instance.objectInfoName.text = placedFeatureSO.displayName;
                    MainSceneUIManager.instance.objectInfoDescription.text = placedFeatureSO.description;
                } else {
                    MainSceneUIManager.instance.objectInfoName.text = placedObjectSO.displayName;
                    foreach (SOUpgrade upgrade in Utils.GetAllAssets<SOUpgrade>()){
                        if (upgrade.IsAvailable(GetComponentInChildren<PlaceableObject>()) && faction == GameManager.instance.Game.PlayerFaction) {
                            MainSceneUIManager.instance.objectInfoDescription.text = $"{placedObjectSO.description}\n\nClick for Upgrades";
                            break;
                        } else {
                            MainSceneUIManager.instance.objectInfoDescription.text = placedObjectSO.description;
                        }
                    }
                }
            }
        }

        if (rangeFinding) target_height = GameManager.instance.PlotMouseOverHeight / 2;

        if (target_height != 0) {
            MainSceneUIManager.instance.plotInfoPanel.SetActive(true);
            if (placedObjectType != null) {
                MainSceneUIManager.instance.objectInfoPanel.SetActive(true);
            }
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, target_height, transform.position.z),
            GameManager.instance.PlotMouseOverSpeed * Time.deltaTime
        );

        if (
            faction == GameManager.instance.Game.PlayerFaction ||
            (from neighbour in GetNeighbours(square: true) select neighbour.faction).Contains(GameManager.instance.Game.PlayerFaction)
        ) {
            SetVisibleToPlayer(true);
        }
    }
}
