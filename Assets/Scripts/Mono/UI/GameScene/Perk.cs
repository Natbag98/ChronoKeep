using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Perk : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image mask;

    [HideInInspector] public SOPerk perk;

    public void _Button_PerkButtonClicked() {
        if (perk.Unlockable()) {
            GameManager.instance.Game.skillPoints -= perk.cost;
            GameManager.instance.Game.perksUnlockTracker.UpdateUnlocked(perk);
        }
    }

    private void Update() {
        if (GameManager.instance.Game.perksUnlockTracker.unlocked[perk]) {
            mask.color = new Color(0f, 0f, 0f, 0f);
        } else {
            if (perk.Unlockable()) {
                mask.color = new Color(0f, 0f, 0f, 180f / 255f);
            } else {
                mask.color = new Color(0f, 0f, 0f, 240f / 255f);
            }
        }
    }

    private void Start() {
        GetComponentInChildren<TextMeshProUGUI>().text = perk.displayName;
    }
}
