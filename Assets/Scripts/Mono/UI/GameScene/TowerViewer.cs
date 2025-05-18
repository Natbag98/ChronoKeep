using UnityEngine;
using UnityEngine.UI;

public class TowerViewer : MonoBehaviour {
    [SerializeField] private Image mask;

    [HideInInspector] public SOPlaceableObject placeableObject;

    private void Update() {
        if (GameManager.instance.Game.placeableObjectsUnlockTracker.disovered[placeableObject.name]) {
            mask.color = Color.white;
            if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.TowerViewer) == gameObject) {
                GameSceneUIManager.instance.towerInfo.SetActive(true);
                GameSceneUIManager.instance.towerName.text = placeableObject.displayName;
                GameSceneUIManager.instance.towerDescription.text = placeableObject.description;
            }
        } else {
            mask.color = Color.black;
        }
    }
}
