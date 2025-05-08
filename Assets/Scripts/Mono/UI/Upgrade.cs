using System;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour {
    [Header("References")]
    [SerializeField] public TextMeshProUGUI buttonText;

    [HideInInspector] public SOUpgrade upgrade;

    void Start() {
        MainSceneUIManager.instance.resetUpgrades += ResetUpgrades;
        buttonText.text = upgrade.displayName;
    }

    private void ResetUpgrades(object _, EventArgs __) {
        MainSceneUIManager.instance.resetUpgrades -= ResetUpgrades;
        Destroy(gameObject);
    }
}
