using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public abstract class UIStorageWindow : AnimatedWindow
    {
        [SerializeField] private UIChestItem item;
        [SerializeField] private Transform holder;
        [SerializeField] private List<UIChestItem> spawnedItems = new List<UIChestItem>(30);
        [SerializeField] private GameObject storageIsEmptyText;

        protected IResourcesService resourcesService;

        public void Init(IResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
        }
        
        protected virtual List<RaftStorage.StorageItem> GetStorage()
        {
            return new List<RaftStorage.StorageItem>();
        }
        
        protected virtual int Redraw()
        {
            var items = GetStorage();
            
            for (int i = spawnedItems.Count; i < items.Count + 1; i++)
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
            
            
            storageIsEmptyText.gameObject.SetActive(items.Count == 0);

            return items.Count;
        }
        
        
        public void GetItemFromStorage(ItemObject itemObject)
        {
            var item = GetStorage().Find(x => x.Item == itemObject);

            if (item != null)
            {
                if (resourcesService.GetGlobalEmptySpace(item))
                {
                    if (RemoveItemFromStorage(item))
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
                if (RemoveItemFromStorage(availableItem))
                {
                    resourcesService.AddItemsToAnyRafts(availableItem);
                    Redraw();
                }
            }
        }

        public virtual bool RemoveItemFromStorage(RaftStorage.StorageItem item)
        {
            return true;
        }
        
        public void TakeAllButton()
        {
            var items = GetStorage();
            while (items.Count != 0 && resourcesService.GetEmptySpace() != 0)
            {
                GetItemFromStorage(items[0].Item);
            }
            if (items.Count == 0){
                CloseWindow();
            }
        }
        
    }
}