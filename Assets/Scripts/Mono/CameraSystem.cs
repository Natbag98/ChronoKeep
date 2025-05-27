using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    public static CameraSystem instance;

    [Header("General Data")]
    [SerializeField] private bool useScroll;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform cameraTransform;

    [Header("Perspective Data")]
    [SerializeField] private float pScrollSpeed;
    [SerializeField] private float pZoomSpeed;
    [SerializeField] private float pMaxZoom;
    [SerializeField] private float pMinZoom;
    [SerializeField] private float pXRotation;
    [SerializeField] private float pYPosition;
    [SerializeField] private float pZPosition;

    [Header("Orth Data")]
    [SerializeField] private float oScrollSpeed;
    [SerializeField] private float oZoomSpeed;
    [SerializeField] private float oMaxZoom;
    [SerializeField] private float oMinZoom;
    [SerializeField] private float oXRotation;
    [SerializeField] private float oYPosition;
    [SerializeField] private float oZPosition;

    [HideInInspector] public bool cameraBlocked = false;
    private Camera cameraComponent;

    private void Start() {
        instance = this;
        cameraComponent = GetComponentInChildren<Camera>();
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
        } else if (useScroll) {
            inputZoomDir.y -= scroll;
            inputZoomDir.z += scroll;
        }

        if (cameraComponent.orthographic) {
            cameraTransform.localPosition = new(0, oYPosition, oZPosition);
            cameraTransform.localEulerAngles = new(oXRotation, 0, 0);

            if (scroll == 0f) {
                inputZoomDir *= oZoomSpeed;
            } else{
                inputZoomDir *= oScrollSpeed;
            }

            if (inputZoomDir != Vector3.zero) {
                cameraComponent.orthographicSize = Math.Min(oMinZoom, Math.Max(oMaxZoom, cameraComponent.orthographicSize + inputZoomDir.z * Time.deltaTime * -1));
            }
        } else {
            cameraTransform.localPosition = new(0, pYPosition, pZPosition);
            cameraTransform.localEulerAngles = new(pXRotation, 0, 0);

            if (scroll == 0f) {
                inputZoomDir *= pZoomSpeed;
            } else{
                inputZoomDir *= pScrollSpeed;
            }
            
            // Zoom the camera by moving it along its local forward axis
            Vector3 zoomDir = transform.forward * inputZoomDir.z + transform.up * inputZoomDir.y;
            cameraTransform.position += zoomDir * Time.deltaTime;

            // Clamp camera zoom
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Max(pMaxZoom, cameraTransform.localPosition.y),
                Mathf.Min(-pMaxZoom, cameraTransform.localPosition.z)
            );
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Min(pMinZoom, cameraTransform.localPosition.y),
                Mathf.Max(-pMinZoom, cameraTransform.localPosition.z)
            );
        }
    }
}
