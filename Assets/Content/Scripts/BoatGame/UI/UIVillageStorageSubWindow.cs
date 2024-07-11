using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIVillageStorageSubWindow : AnimatedWindow
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private UIChestItem item;
        [SerializeField] private Transform holder;
        [SerializeField] private GameObject storageIsEmptyText;

        [SerializeField] private List<UIChestItem> spawnedItems = new List<UIChestItem>(30);
        private SlaveDataCalculator slaveDataCalculator;
        private ResourcesService resourcesService;

        public void Init(ResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
        }

        public void OpenSlaveStorage(SlaveDataCalculator slaveDataCalculator)
        {
            this.slaveDataCalculator = slaveDataCalculator;
            
            headerText.text = slaveDataCalculator.CharacterData.Character.Name + " storage";

            Redraw();

            ShowWindow();
        }

        private void Redraw()
        {
            var items = slaveDataCalculator.GetStorage();

            storageIsEmptyText.gameObject.SetActive(items.Count == 0);

            for (int i = spawnedItems.Count; i < items.Count; i++)
            {
                if (spawnedItems.Count < i)
                {
                    Instantiate(item, holder).With(x => spawnedItems.Add(x));
                }
            }

            for (int i = 0; i < spawnedItems.Count; i++)
            {
                if (items.Count > i)
                {
                    spawnedItems[i].Init(null, items[i].Item, items[i].Count);
                    spawnedItems[i].BindButton(GetItemFromStorage);
                    spawnedItems[i].gameObject.SetActive(true);
                }
                else
                {
                    spawnedItems[i].gameObject.SetActive(false);
                }
            }
        }

        public void GetItemFromStorage(ItemObject itemObject)
        {
            var item = slaveDataCalculator.GetStorage().Find(x => x.Item == itemObject);

            if (item != null)
            {
                if (resourcesService.GetGlobalEmptySpace(item))
                {
                    if (slaveDataCalculator.RemoveItem(item))
                    {
                        resourcesService.AddItemsToAnyRafts(item);
                        Redraw();
                    }

                    return;
                }
                
                var empty = resourcesService.GetEmptySpace();
                if (empty == 0) return;

                if (empty > item.Count)
                {
                    empty = item.Count;
                }

                var availableItem = new RaftStorage.StorageItem(item.Item, empty);
                if (slaveDataCalculator.RemoveItem(availableItem))
                {
                    resourcesService.AddItemsToAnyRafts(availableItem);
                    Redraw();
                }
            }
        }
    }
}
