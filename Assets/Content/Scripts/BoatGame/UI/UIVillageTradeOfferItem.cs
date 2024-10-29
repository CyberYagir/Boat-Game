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
            private float modify = 1;

            public RaftStorage.StorageItem Item => offerItem;

            public void Init(RaftStorage.StorageItem offerItem)
            {
                this.offerItem = offerItem;
                icon.sprite = offerItem.Item.ItemIcon;
                SetModify(modify);
            }


            public void SetModify(float getActualModify)
            {
                modify = getActualModify;
                text.text = offerItem.Count <= 1 ? "" : $"x{GetCost()}";
            }

            public RaftStorage.StorageItem GetStorageItem()
            {
                return new RaftStorage.StorageItem(Item.Item, GetCost());
            }

            private int GetCost()
            {
                var value = Mathf.RoundToInt(offerItem.Count * modify);
                if (value <= 0) return 1;
                return value;
            }
        }

        [SerializeField] private OfferItem sellItem, resultItem;
        [SerializeField] private UICustomButton button;
        private TradeOfferObject tradeOfferObject;
        private UIVillageOptionsWindow window;
        private IResourcesService resourcesService;

        public void Init(TradeOfferObject tradeOfferObject, UIVillageOptionsWindow window, IResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            this.window = window;
            this.tradeOfferObject = tradeOfferObject;

            sellItem.Init(tradeOfferObject.SellItem);
            resultItem.Init(tradeOfferObject.ResultItem);

            var modify = window.GetActualModify();
            resultItem.SetModify(modify);
        }


        public void Sell() => window.SellItem(sellItem.GetStorageItem(), resultItem.GetStorageItem(), tradeOfferObject);

        public void UpdateItem(float getActualModify)
        {
            resultItem.SetModify(getActualModify);
            button.SetInteractable(resourcesService.IsCanTradeItem(sellItem.GetStorageItem(), resultItem.GetStorageItem()));
        }
    }
}
