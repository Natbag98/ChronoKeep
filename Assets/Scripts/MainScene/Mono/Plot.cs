using System;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour {
    public static int neighbourUp = 0;
    public static int neighbourRight = 1;
    public static int neighbourDown = 2;
    public static int neighbourLeft = 3;

    [SerializeField] private bool canPlaceObject;

    [HideInInspector] public GameManager.PlaceableObjectTypes? placedObjectType = null;
    private Faction faction;
    private Plot[] neighbours;
    private bool mouseOver;

    public bool GetCanPlaceObject() { return canPlaceObject; }
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

    public void SetNeighbours(Plot[] neighbours) { this.neighbours = neighbours; }
    public void SetFaction(Faction faction) { this.faction = faction; }

    public bool CanCharacterMoveThrough() {
        if (placedObjectType != null) {
            return false;
        } else {
            return canPlaceObject;
        }
    }

    public void PlaceObject(SOPlaceableObject object_to_place) {
        MainSceneUIManager.instance.ObjectPlaced();
        PlaceableObject new_object = Instantiate(
            object_to_place.placeableObjectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        ).GetComponent<PlaceableObject>();
        placedObjectType = new_object.objectType = object_to_place.objectType;
    }

    public Vector2Int GetPositionInPlotArray() {
        return new(
            (int)transform.position.x + GameManager.instance.Game.TerrainSize.x / 2,
            (int)transform.position.z + GameManager.instance.Game.TerrainSize.y / 2
        );
    }

    public void OnMouseEnter() {
        mouseOver = true;
    }

    public void OnMouseExit() {
        mouseOver = false;
    }

    private void OnMouseDown() {
        if (MainSceneUIManager.instance.IsPlacingObject() && canPlaceObject) {
            PlaceObject(MainSceneUIManager.instance.GetObjectToPlace());
        }
    }

    private void Update() {
        float target_height;
        if (mouseOver) {
            target_height = GameManager.instance.PlotMouseOverHeight;
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
