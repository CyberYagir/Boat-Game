using System;
using System.Collections.Generic;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public abstract class ResourcesServiceBase : MonoBehaviour
    {
        [SerializeField] protected Dictionary<ItemObject, int> allItemsList = new Dictionary<ItemObject, int>(20);
        private bool isAnyRaftBeenChanged = false;
        public virtual event Action OnChangeResources;
        public event Action<ItemObject> OnAddItemToRaft; 
        public Dictionary<ItemObject, int> AllItemsList => allItemsList;

        protected void OnAnyStorageChange()
        {
            isAnyRaftBeenChanged = true;
            PlayerItemsList();
            OnChangeResources?.Invoke();
        }

        public virtual List<RaftStorage> GetRafts() => null;

        public void PlayerItemsList()
        {
            if (!isAnyRaftBeenChanged) return;
            
            allItemsList.Clear();
            var storages = GetRafts();
            foreach (var raftStorage in storages)
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

            var storage = GetRafts().Find(x => x.HaveItem(itemObject));

            if (storage != null)
            {
                storage.RemoveFromStorage(itemObject);
            }
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
        
        public  void AddItemsToAnyRafts(RaftStorage.StorageItem oldItem, bool spawnPopup = true)
        {
            var storages = GetRafts();
            if (oldItem.Item.HasSize)
            {
                for (int j = 0; j < storages.Count; j++)
                {
                    if (oldItem.Count <= 0) return;

                    var empty = storages[j].GetEmptySlots();

                    if (empty > oldItem.Count)
                    {
                        empty = oldItem.Count;
                    }
                    
                    storages[j].AddToStorage(oldItem.Item, empty, spawnPopup);
                    
                    oldItem.Add(-empty);
                }
            }
            else
            {
                if (storages.Count != 0)
                {
                    var randomStorage = storages.GetRandomItem();
                    randomStorage.AddToStorage(oldItem.Item, oldItem.Count, spawnPopup);
                }
            }
            OnAddItemToRaft?.Invoke(oldItem.Item);
        }
        
        public bool GetGlobalEmptySpace(RaftStorage.StorageItem storageItem, int offcet = 0)
        {
            if (!storageItem.Item.HasSize) return true;
            return GetEmptySpace() + offcet >= storageItem.Count;
        }

        public int GetEmptySpace()
        {
            int emptySpace = 0;
            var storages = GetRafts();
            foreach (var storage in storages)
            {
                emptySpace += storage.GetEmptySlots();
            }

            return emptySpace;
        }
        
        public bool TrySwapItems(RaftStorage.StorageItem newItem, RaftStorage.StorageItem oldItem)
        {
            var calculateSpace = GetEmptySpace() - newItem.Count + oldItem.Count;
            var maxItemsCount = 0;
            var storages = GetRafts();
            for (int i = 0; i < storages.Count; i++)
            {
                maxItemsCount += storages[i].MaxItemsCount;
            }

            if (calculateSpace <= maxItemsCount)
            {
                RemoveItemsFromAnyRaft(newItem);
                AddItemsToAnyRafts(oldItem);
                return true;
            }

            return false;
        }
        
        public void RemoveItemsFromAnyRaft(RaftStorage.StorageItem storageItem)
        {
            var storages = GetRafts();
            for (int i = 0; i < storages.Count; i++)
            {
                if (storageItem.Count <= 0) break;
                if (storages[i].HaveItem(storageItem.Item))
                {
                    var items = storages[i].GetItem(storageItem.Item);

                    if (storageItem.Count <= items.Count)
                    {
                        storages[i].RemoveFromStorage(storageItem.Item, storageItem.Count);
                        return;
                    }

                    if (storageItem.Count > items.Count)
                    {
                        storages[i].RemoveFromStorage(storageItem.Item, items.Count);
                        storageItem.Add(-items.Count);
                    }
                }
            }
        }
        
        public virtual bool AddToAnyStorage(ItemObject item)
        {
            var storages = GetRafts();
            var storage = storages.Find(x => x.HaveItem(item) && x.GetEmptySlots() >= 1);
            if (storage != null)
            {
                storage.AddToStorage(item, 1);
                OnAddItemToRaft?.Invoke(item);
                return true;
            }
            else
            {
                foreach (var raftStorage in storages)
                {
                    if (raftStorage.IsEmptyStorage(item, 1))
                    {
                        raftStorage.AddToStorage(item, 1);
                        OnAddItemToRaft?.Invoke(item);
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
    }
}