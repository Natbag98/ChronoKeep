using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomObjectCollapse", menuName = "Event/RandomObjectCollapse")]
public class RandomObjectCollapse : SOEvent {
    [Header("Random Object Collapse")]
    [SerializeField] private bool onPlayer;

    private List<Plot> destroy;

    public override bool IsValid() {
        if (onPlayer) {
            destroy = new();
            foreach (Plot plot in RunManager.instance.GetAllPlotsWithFactionObjects(GameManager.instance.Game.PlayerFaction)) {
                if (plot.GetComponentInChildren<PlaceableObject>().placeableObjectSO.objectType != GameManager.PlaceableObjectTypes.Castle) destroy.Add(plot);
            }
            return destroy.Count != 0;
        } else {
            throw new System.NotImplementedException();
        }
    }

    public override void Event() {
        if (onPlayer) {
            Utils.Choice(destroy).GetComponentInChildren<PlaceableObject>().DestroySelf();
        } else {
            throw new System.NotImplementedException();
        }
    }
}
