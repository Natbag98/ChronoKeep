using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;

    private SOPlaceableObject objectToBuy;

    private void Start() {
        objectToBuy = Utils.Choice(Utils.GetAllAssets<SOPlaceableObject>());
        text.text = objectToBuy.displayName;
    }

    public void _Button_ButtonClicked() {
        if (GameManager.instance.Game.SpendResources(objectToBuy.purchaseCost.GetDict())) {
            MainSceneUIManager.instance.PlaceInventoryItem(objectToBuy);
            Destroy(gameObject);
        }
    }
}
