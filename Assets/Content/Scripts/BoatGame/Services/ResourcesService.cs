using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class ResourcesService : ResourcesServiceBase, IResourcesService
    {
        private RaftBuildService raftBuildService;

        public override event Action OnChangeResources;

        [Inject]
        private void Construct(RaftBuildService raftBuildService, GameDataObject gameData)
        {
            this.raftBuildService = raftBuildService;

            raftBuildService.OnChangeRaft += OnRaftsChanges;

            OnRaftsChanges();
        }

        private void OnRaftsChanges()
        {
            foreach (var spawnedRaft in raftBuildService.Storages)
            {
                spawnedRaft.OnStorageChange -= OnAnyStorageChange;
                spawnedRaft.OnStorageChange += OnAnyStorageChange;
            }
        }

        public override List<RaftStorage> GetRafts() => raftBuildService.Storages;
        

        public void RemoveItemsFromAnyRaft(RaftStorage.StorageItem storageItem)
        {
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                if (storageItem.Count <= 0) break;
                if (raftBuildService.Storages[i].HaveItem(storageItem.Item))
                {
                    var items = raftBuildService.Storages[i].GetItem(storageItem.Item);

                    if (storageItem.Count <= items.Count)
                    {
                        raftBuildService.Storages[i].RemoveFromStorage(storageItem.Item, storageItem.Count);
                        return;
                    }

                    if (storageItem.Count > items.Count)
                    {
                        raftBuildService.Storages[i].RemoveFromStorage(storageItem.Item, items.Count);
                        storageItem.Add(-items.Count);
                    }
                }
            }
        }

        public bool TrySwapItems(RaftStorage.StorageItem newItem, RaftStorage.StorageItem oldItem)
        {
            var calculateSpace = GetEmptySpace() - newItem.Count + oldItem.Count;
            var maxItemsCount = 0;
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                maxItemsCount += raftBuildService.Storages[i].MaxItemsCount;
            }

            if (calculateSpace <= maxItemsCount)
            {
                RemoveItemsFromAnyRaft(newItem);
                AddItemsToAnyRafts(oldItem);
                return true;
            }

            return false;
        }

        public bool TrySwapItemsWithDrop(RaftStorage.StorageItem newItem, RaftStorage.StorageItem oldItem)
        {
            var calculateSpace = GetEmptySpace() + oldItem.Count;
            var maxItemsCount = 0;
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                maxItemsCount += raftBuildService.Storages[i].MaxItemsCount;
            }

            if (calculateSpace <= maxItemsCount)
            {
                AddItemsToAnyRafts(oldItem);
                return true;
            }

            return false;
        }



        public bool AddToAnyStorage(ItemObject item)
        {
            var storage = raftBuildService.Storages.Find(x => x.HaveItem(item) && x.GetEmptySlots() >= 1);
            if (storage != null)
            {
                storage.AddToStorage(item, 1);
                return true;
            }
            else
            {
                foreach (var raftStorage in raftBuildService.Storages)
                {
                    if (raftStorage.IsEmptyStorage(item, 1))
                    {
                        raftStorage.AddToStorage(item, 1);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsHaveItem(RaftStorage.StorageItem sellItem)
        {
            if (allItemsList.ContainsKey(sellItem.Item))
            {
                return allItemsList[sellItem.Item] >= sellItem.Count;
            }

            return false;
        }
        public bool IsHaveItem(ItemObject item, int count)
        {
            return IsHaveItem(new RaftStorage.StorageItem(item, count));
        }

        private List<RaftStorage> emptyStoragesArray = new List<RaftStorage>(10);
        public List<RaftStorage> FindEmptyStorages(ItemObject item, int value)
        {
            emptyStoragesArray.Clear();
            foreach (var raftStorage in raftBuildService.Storages)
            {
                if (raftStorage.IsEmptyStorage(item, value))
                {
                    emptyStoragesArray.Add(raftStorage);
                }
            }
            return emptyStoragesArray;
        }
        
        public RaftStorage FindStorageByResource(EResourceTypes type)
        {
            return raftBuildService.Storages.Find(x => x.GetResourceByType(type) > 0);
        }
        
        
        
        public bool HaveMaterialsForCrafting(List<CraftObject.CraftItem> currentCraftIngredients)
        {
            foreach (var currentCraftIngredient in currentCraftIngredients)
            {
                if (allItemsList[currentCraftIngredient.ResourceName] < currentCraftIngredient.Count)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsCanTradeItem(RaftStorage.StorageItem sellItem, RaftStorage.StorageItem resultItem)
        {
            var isCan = IsHaveItem(sellItem) && GetEmptySpace() + sellItem.Space >= resultItem.Space;
            return isCan;
        }
        

        public int GetItemsValue(ItemObject resourceName)
        {
            if (allItemsList.ContainsKey(resourceName))
            {
                return allItemsList[resourceName];
            }
            
            return 0;
        }
    }
}
