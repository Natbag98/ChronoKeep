using UnityEngine;
using UnityEngine.UI;

public class TowerViewer : MonoBehaviour {
    [SerializeField] private Image mask;

    [HideInInspector] public SOPlaceableObject placeableObject;

    private void Update() {
        if (GameManager.instance.Game.placeableObjectsUnlockTracker.disovered[placeableObject]) {
            mask.color = Color.white;
            if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.TowerViewer) == gameObject) {
                Utils.GetManager<GameSceneUIManager>().towerInfo.SetActive(true);
                Utils.GetManager<GameSceneUIManager>().towerName.text = placeableObject.displayName;
                Utils.GetManager<GameSceneUIManager>().towerDescription.text = placeableObject.description;
            }
        } else {
            mask.color = Color.black;
        }
    }
}
