using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftStorage : MonoBehaviour
    {
        
        [System.Serializable]
        public class StorageItem
        {
            [SerializeField] private ItemObject item;
            [SerializeField] private int count;

            public StorageItem(ItemObject item, int count)
            {
                this.item = item;
                this.count = count;
            }

            public int Count => count;

            public ItemObject Item => item;

            public void Add(int value)
            {
                count += value;
            }
        }
        
        [SerializeField] private List<StorageItem> items = new List<StorageItem>();
        [SerializeField] private int maxItemsCount;
        
        
        public Action OnStorageChange;

        public List<StorageItem> Items => items;


        public StorageItem GetItem(ItemObject item)
        {
            return Items.Find(x => x.Item.ID == item.ID);
        }
        
        
        public List<StorageItem> GetItem(EResourceTypes name)
        {
            return Items.FindAll(x => x.Item.Type == name && x.Count > 0);
        }


        public bool IsEmptyStorage(int value)
        {
            return items.Sum(x=>x.Count) + value <= maxItemsCount;
        }

        public int GetResourceByType(EResourceTypes type)
        {
            var item = Items.Find(x => x.Item.Type == type);
            if (item != null)
            {
                return item.Count;
            }

            return 0;
        }

        public void AddToStorage(ItemObject item, int resourceDataValue, bool spawnPopup = true)
        {
            var storage = GetItem(item);
            if (storage != null)
            {
                storage.Add(resourceDataValue);
            }
            else
            {
                items.Add(new StorageItem(item, resourceDataValue));
            }

            if (spawnPopup)
            {

                WorldPopupService.StaticSpawnPopup(
                    transform.position,
                    item,
                    resourceDataValue
                );
            }

            OnStorageChange?.Invoke();
        }

        public bool HaveItem(ItemObject item)
        {
            var itemInStorage = GetItem(item);
            if (itemInStorage == null) return false;
            if (itemInStorage.Count == 0) return false;
            
            return true;
        }

        public ItemObject RemoveFromStorage(EResourceTypes type)
        {
            var storage = GetItem(type);

            if (storage.Count == 0) return null;
            
            var remove = storage.First();
            
            if (remove != null)
            {
                RemoveItem(remove);
                return remove.Item;
            }
            return null;
        }

        private void RemoveItem(StorageItem remove)
        {
            remove.Add(-1);

            if (remove.Count == 0)
            {
                items.Remove(remove);
            }
            OnStorageChange?.Invoke();
        }

        public bool RemoveFromStorage(ItemObject item)
        {
            StorageItem remove = GetItem(item);
            
            if (remove != null)
            {
                RemoveItem(remove);
                return true;
            }

            return false;
        }

        public void LoadStorage(SaveDataObject.RaftsData.RaftStorage data, GameDataObject gameData)
        {
            items.Clear();
            for (int i = 0; i < data.StoragesData.Count; i++)
            {
                if (data.StoragesData[i].Count > 0)
                {
                    var item = gameData.GetItem(data.StoragesData[i].ItemID);
                    var storageItems = new StorageItem(item, data.StoragesData[i].Count);
                    items.Add(storageItems);
                    OnStorageChange?.Invoke();
                }
            }
            
        }

        public void RemoveAllFromStorage()
        {
            Items.Clear();
            OnStorageChange?.Invoke();
        }

        public int GetEmptySlots()
        {
            return maxItemsCount - items.Sum(x=>x.Count);
        }
    }
}
