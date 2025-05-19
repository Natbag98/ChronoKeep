using UnityEngine;
using UnityEngine.UI;

public class CharacterViewer : MonoBehaviour {
    [SerializeField] private Image mask;

    [HideInInspector] public SOCharacter character;

    private void Update() {
        if (GameManager.instance.Game.characterUnlockTracker.disovered[character]) {
            mask.color = Color.white;
            if (Utils.CheckMouseHoveringOverUIElementWithTag(Tag.Tags.CharacterViewer) == gameObject) {
                GameSceneUIManager.instance.characterInfo.SetActive(true);
                GameSceneUIManager.instance.characterName.text = character.displayName;
                GameSceneUIManager.instance.characterDescription.text = character.description;
            }
        } else {
            mask.color = Color.black;
        }
    }
}
