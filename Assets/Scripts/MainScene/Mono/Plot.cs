using UnityEngine;

public class Plot : MonoBehaviour {
    private bool mouseOver;

    public void OnMouseEnter() {
        mouseOver = true;
        Debug.Log("Over");
    }

    public void OnMouseExit() {
        mouseOver = false;
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
