using UnityEngine;

public class ColorCube : MonoBehaviour {
    [SerializeField] private Plot plot;
    [SerializeField] private MeshRenderer mesh;

    void Start() {
        mesh.material.color = new Color((float)GameManager.Random.NextDouble(), mesh.material.color.g, mesh.material.color.b, (float)GameManager.Random.NextDouble());
    }

    void Update() {
        if (plot.faction == null) {
            mesh.material.color = new(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, 0);
        } else {
            
        }
    }
}
