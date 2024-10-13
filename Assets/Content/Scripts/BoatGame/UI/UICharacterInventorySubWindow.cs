using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterInventorySubWindow : DragAreaWindow
    {
        [SerializeField] private UIInventoryItem item;
        [SerializeField] private GameObject itemsNotFoundText;
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private IRaftBuildService raftBuildService;


        public void Init(IRaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            Redraw();
        }
        public void Redraw()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].DisableItem();
                Destroy(items[i].gameObject);
            }

            items.Clear();
            item.gameObject.SetActive(true);
            int count = 0;
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetItem(EResourceTypes.Other);

                if (other != null)
                {
                    foreach (var otherItemObject in other)
                    {
                        for (int i = 0; i < otherItemObject.Count; i++)
                        {
                            Instantiate(item, item.transform.parent)
                                .With(x => x.Init(otherItemObject.Item, this))
                                .With(x => items.Add(x));
                            count++;
                        }
                    }
                }
            }

            itemsNotFoundText.gameObject.SetActive(count == 0);
            item.gameObject.SetActive(false);
        }
    }
}
