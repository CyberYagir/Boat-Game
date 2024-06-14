using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageTradeOfferItem : MonoBehaviour
    {
        [System.Serializable]
        public class OfferItem
        {
            [SerializeField] private Image icon;
            [SerializeField] private TMP_Text text;
            private RaftStorage.StorageItem offerItem;

            public RaftStorage.StorageItem Item => offerItem;

            public void Init(RaftStorage.StorageItem offerItem)
            {
                this.offerItem = offerItem;
                icon.sprite = offerItem.Item.ItemIcon;
                text.text = offerItem.Count <= 1 ? "" : $"x{offerItem.Count}";
            }
        }

        [SerializeField] private OfferItem sellItem, resultItem;
        [SerializeField] private UICustomButton button;
        private TradeOfferObject tradeOfferObject;
        private UIVillageOptionsWindow window;
        private ResourcesService resourcesService;

        public void Init(TradeOfferObject tradeOfferObject, UIVillageOptionsWindow window, ResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            this.window = window;
            this.tradeOfferObject = tradeOfferObject;
            
            sellItem.Init(tradeOfferObject.SellItem);
            resultItem.Init(tradeOfferObject.ResultItem);
        }


        public void Sell() => window.SellItem(tradeOfferObject);

        public void UpdateItem()
        {
            button.SetInteractable(resourcesService.IsHaveItem(sellItem.Item) && resourcesService.GetEmptySpace() - sellItem.Item.Count >= resultItem.Item.Count);
        }
    }
}
