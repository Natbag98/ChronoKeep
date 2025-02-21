using UnityEngine;

public class RotateCanvas : MonoBehaviour {
    private void Update() {
        Utils.RotateTowards(transform.position, Camera.allCameras[0].transform.position, transform);
    }
}
