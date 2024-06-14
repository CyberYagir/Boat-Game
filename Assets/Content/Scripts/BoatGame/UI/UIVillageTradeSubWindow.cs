using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Scriptable;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageTradeSubWindow : MonoBehaviour
    {
        [System.Serializable]
        public class OffersDrawer
        {
            [SerializeField] private UIVillageTradeOfferItem item;
            private List<TradeOfferObject> offersList;
            private List<UIVillageTradeOfferItem> spawned = new List<UIVillageTradeOfferItem>();

            public void Init(List<TradeOfferObject> items, UIVillageOptionsWindow window, ResourcesService resourcesService)
            {
                offersList = items;
                item.gameObject.SetActive(true);
                
                foreach (var it in spawned)
                {
                    Destroy(it.gameObject);
                }
                
                spawned.Clear();
                
                for (int i = 0; i < offersList.Count; i++)
                {
                    var id = i;
                    Instantiate(item, item.transform.parent)
                        .With(x => x.Init(offersList[id], window, resourcesService))
                        .With(x => spawned.Add(x));
                }

                item.gameObject.SetActive(false);
            }

            public void UpdateItems()
            {
                for (int i = 0; i < spawned.Count; i++)
                {
                    spawned[i].UpdateItem();
                }
            }
        }

        [SerializeField] private OffersDrawer sellItemsDrawer, buyItemsDrawer;

        public void Init(
            List<TradeOfferObject> sellOffers,
            List<TradeOfferObject> buyOffers,
            UIVillageOptionsWindow window,
            ResourcesService resourcesService
        )
        {
            sellItemsDrawer.Init(sellOffers, window, resourcesService);
            buyItemsDrawer.Init(buyOffers, window, resourcesService);
            resourcesService.OnChangeResources -= UpdateItems;
            resourcesService.OnChangeResources += UpdateItems;
        }

        private void UpdateItems()
        {
            buyItemsDrawer.UpdateItems();
            sellItemsDrawer.UpdateItems();
        }

        private void OnEnable()
        {
            Redraw();
        }

        public void Redraw()
        {
            UpdateItems();
        }
    }
}
