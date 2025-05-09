using System;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour {
    [Header("References")]
    [SerializeField] public TextMeshProUGUI buttonText;

    [HideInInspector] public SOUpgrade upgrade;

    public void _Button_UpgradeButtonClicked() {
        GameManager.instance.Game.SpendResources(upgrade.cost.GetDict());
        MainSceneUIManager.instance.upgradePlot.GetComponentInChildren<PlaceableObject>().AddUpgrade(upgrade);
        MainSceneUIManager.instance.resetUpgrades -= ResetUpgrades;
        Destroy(gameObject);
    }

    void Start() {
        MainSceneUIManager.instance.resetUpgrades += ResetUpgrades;
        buttonText.text = upgrade.displayName;
    }

    private void ResetUpgrades(object _, EventArgs __) {
        MainSceneUIManager.instance.resetUpgrades -= ResetUpgrades;
        Destroy(gameObject);
    }
}
