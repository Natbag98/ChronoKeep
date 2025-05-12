using UnityEngine;

public class CameraSystem : MonoBehaviour {
    public static CameraSystem instance;

    [SerializeField] private bool useScroll;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Transform cameraTransform;

    public bool cameraBlocked = false;

    private void Start() {
        instance = this;
    }

    private void Update() {
        if (cameraBlocked) return;

        // Move the camera on the x and z axes
        Vector3 inputDir = new(0f, 0f, 0f);
        if (Input.GetKey(KeyCode.W)) inputDir.z += 1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z -= 1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x -= 1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x += 1f;
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveSpeed * Time.deltaTime * moveDir;

        // Rotate the camera
        float rotationDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationDir += 1f;
        if (Input.GetKey(KeyCode.E)) rotationDir -= 1f;
        transform.eulerAngles = new(
            transform.eulerAngles.x,
            transform.eulerAngles.y + rotationDir * rotateSpeed * Time.deltaTime,
            transform.eulerAngles.z
        );

        // Zoom the camera by moving it along its local forward axis
        Vector3 inputZoomDir = new(0f, 0f, 0f);
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) {
            if (Input.GetKey(KeyCode.R)) {
                inputZoomDir.y -= 1f;
                inputZoomDir.z += 1f;
            }
            if (Input.GetKey(KeyCode.F)) {
                inputZoomDir.y += 1f;
                inputZoomDir.z -= 1f;
            }
            inputZoomDir *= zoomSpeed;
        } else if (useScroll) {
            inputZoomDir.y -= scroll;
            inputZoomDir.z += scroll;
            inputZoomDir *= scrollSpeed;
        }
        Vector3 zoomDir = transform.forward * inputZoomDir.z + transform.up * inputZoomDir.y;
        cameraTransform.position += zoomDir * Time.deltaTime;

        // Clamp camera zoom
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            Mathf.Max(maxZoom, cameraTransform.localPosition.y),
            Mathf.Min(-maxZoom, cameraTransform.localPosition.z)
        );
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            Mathf.Min(minZoom, cameraTransform.localPosition.y),
            Mathf.Max(-minZoom, cameraTransform.localPosition.z)
        );
    }
}
