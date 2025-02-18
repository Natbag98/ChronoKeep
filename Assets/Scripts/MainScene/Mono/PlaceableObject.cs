using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour, IRangedTarget {
    [Header("PlaceableObject")]
    [SerializeField] protected Attributes attributes;
    [SerializeField] private Transform centerPoint;

    [HideInInspector] public GameManager.PlaceableObjectTypes objectType;
    [HideInInspector] public Plot parentPlot;

    public Vector3 GetTargetPoint() { return centerPoint.position; }

    protected List<Plot> GetPlotsInRange() { return parentPlot.GetNeighbours(attributes.GetAttributeAsInt(GameManager.Attributes.Range)); }
}
