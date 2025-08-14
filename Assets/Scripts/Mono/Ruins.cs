using System.Linq;
using UnityEngine;

public class Ruins : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject defaultRuins;
    [SerializeField] Utils.SerializeableDict<string, GameObject> ruins;

    [HideInInspector] public string placeableObjectName;

    private void Start() {
        if (ruins.GetDict().Keys.Contains(placeableObjectName)) {
            Instantiate(ruins.GetDict()[placeableObjectName], transform);
        } else {
            Instantiate(defaultRuins, transform);
        }
    }
}
