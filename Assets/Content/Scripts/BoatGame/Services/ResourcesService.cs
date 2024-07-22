using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{

    public enum EResourceTypes
    {
        Eat,
        Water,
        Build,
        Money,
        Other,
        Potions
    }

    public class ResourcesService : MonoBehaviour
    {
        [SerializeField] private Dictionary<ItemObject, int> allItemsList = new Dictionary<ItemObject, int>(20);

        private RaftBuildService raftBuildService;

        public event Action OnChangeResources;
        public Dictionary<ItemObject, int> AllItemsList => allItemsList;

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

        private bool isAnyRaftBeenChanged = false;
        private void OnAnyStorageChange()
        {
            isAnyRaftBeenChanged = true;
            PlayerItemsList();
            OnChangeResources?.Invoke();
        }


        public void PlayerItemsList()
        {
            if (!isAnyRaftBeenChanged) return;
            
            allItemsList.Clear();
            foreach (var raftStorage in raftBuildService.Storages)
            {
                for (int i = 0; i < raftStorage.Items.Count; i++)
                {
                    if (allItemsList.ContainsKey(raftStorage.Items[i].Item))
                    {
                        allItemsList[raftStorage.Items[i].Item] += raftStorage.Items[i].Count;
                    }
                    else
                    {
                        
                        allItemsList.Add(raftStorage.Items[i].Item, raftStorage.Items[i].Count);
                    }
                }
            }
            
            isAnyRaftBeenChanged = false;
        }

        public void RemoveItemFromAnyRaft(ItemObject itemObject)
        {
            if (itemObject == null) return;

            var storage = raftBuildService.Storages.Find(x => x.HaveItem(itemObject));

            if (storage != null)
            {
                storage.RemoveFromStorage(itemObject);
            }
        }

        public bool GetGlobalEmptySpace(RaftStorage.StorageItem storageItem, int offcet = 0)
        {
            if (!storageItem.Item.HasSize) return true;
            return GetEmptySpace() + offcet >= storageItem.Count;
        }

        public int GetEmptySpace()
        {
            int emptySpace = 0;
            foreach (var storage in raftBuildService.Storages)
            {
                emptySpace += storage.GetEmptySlots();
            }

            return emptySpace;
        }

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

        public void AddItemsToAnyRafts(RaftStorage.StorageItem oldItem)
        {
            if (oldItem.Item.HasSize)
            {
                for (int j = 0; j < raftBuildService.Storages.Count; j++)
                {
                    if (oldItem.Count <= 0) return;

                    var empty = raftBuildService.Storages[j].GetEmptySlots();

                    if (empty > oldItem.Count)
                    {
                        empty = oldItem.Count;
                    }
                    
                    raftBuildService.Storages[j].AddToStorage(oldItem.Item, empty);
                    
                    oldItem.Add(-empty);
                    // while (raftBuildService.Storages[j].IsEmptyStorage(oldItem.Item, 1))
                    // {
                    //     raftBuildService.Storages[j].AddToStorage(oldItem.Item, 1);
                    //     oldItem.Add(-1);
                    //     if (oldItem.Count <= 0) return;
                    // }
                }
            }
            else
            {
                if (raftBuildService.Storages.Count != 0)
                {
                    var randomStorage = raftBuildService.Storages.GetRandomItem();
                    randomStorage.AddToStorage(oldItem.Item, oldItem.Count);
                }
            }
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

        private List<RaftStorage.StorageItem> tmpSearchList = new List<RaftStorage.StorageItem>(20);
        public List<RaftStorage.StorageItem> GetItemsByType(EResourceTypes Type)
        {
            tmpSearchList.Clear();
            foreach (var val in allItemsList)
            {
                if (val.Key.Type == Type)
                {
                    if (val.Value > 0)
                    {
                        tmpSearchList.Add(new RaftStorage.StorageItem(val.Key, val.Value));
                    }
                }
            }

            return tmpSearchList;
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
