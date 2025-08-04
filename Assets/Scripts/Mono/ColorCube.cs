using UnityEngine;

public class ColorCube : MonoBehaviour {
    [SerializeField] private Plot plot;
    [SerializeField] private MeshRenderer mesh;

    private bool visible = false;

    void Start() {
        plot = GetComponentInParent<Plot>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.G)) {
            visible = true;
        } else {
            visible = false;
        }

        if (plot.faction == null || !visible) {
            mesh.material.color = new(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, 0f);
        }  else {
            mesh.material.color = plot.faction.Color;
        }
    }
}
