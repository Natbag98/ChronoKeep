using UnityEngine;

public class Plot : MonoBehaviour {
    private bool mouseOver;

    private void PlaceObject(SOPlaceableObject object_to_place) {
        MainSceneUIManager.instance.ObjectPlaced();
        PlaceableObject new_object = Instantiate(
            object_to_place.placeableObjectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        ).GetComponent<PlaceableObject>();
        new_object.objectType = object_to_place.objectType;
    }

    public void OnMouseEnter() {
        mouseOver = true;
    }

    public void OnMouseExit() {
        mouseOver = false;
    }

    private void OnMouseDown() {
        if (MainSceneUIManager.instance.IsPlacingObject()) {
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
