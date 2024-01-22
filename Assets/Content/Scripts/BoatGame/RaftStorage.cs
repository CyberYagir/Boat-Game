using System;
using System.Collections.Generic;
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
        public class ResourceTypeHolder
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

            [SerializeField] private EResourceTypes resourceType;
            [SerializeField] private int maxCount;

            [SerializeField] private List<StorageItem> itemObjects = new List<StorageItem>();

            public EResourceTypes ResourcesType => resourceType;

            
            public ResourceTypeHolder(EResourceTypes resourceType, int maxCount)
            {
                this.resourceType = resourceType;
                this.maxCount = maxCount;
            }


            public int Count => GetCount();

            public int MaxCount => maxCount;

            public List<StorageItem> ItemObjects => itemObjects;


            public void SetMaxCount(int count)
            {
                maxCount = count;
            }
            
            public bool IsCanFitItem(ItemObject itemObject, int value)
            {
                return GetCount() + value <= MaxCount;
            }


            public int GetCount()
            {
                int count = 0;
                for (int i = 0; i < ItemObjects.Count; i++)
                {
                    count += ItemObjects[i].Count;
                }

                return count;
            }

            public void Add(ItemObject itemObject, int value)
            {
                var findedItem = ItemObjects.Find(x => x.Item.ID == itemObject.ID);

                if (findedItem == null)
                {
                    ItemObjects.Add(new StorageItem(itemObject, value));
                }
                else
                {
                    findedItem.Add(value);
                }
                
            }

            public void RemoveItem(ItemObject itemObject)
            {
                if (ItemObjects.Count == 0) return;

                var find = ItemObjects.Find(x => x.Item.ID == itemObject.ID);

                if (find != null)
                {
                    find.Add(-1);
                }

                if (find.Count == 0)
                {
                    ItemObjects.Remove(find);
                }
            }
            
            public ItemObject RemoveItem()
            {
                if (ItemObjects.Count == 0) return null;
                
                var holder = ItemObjects[0];
                holder.Add(-1);
                if (holder.Count == 0)
                {
                    ItemObjects.RemoveAt(0);
                }
                
                return holder.Item;
            }

            public int GetEmptySlots()
            {
                return maxCount - Count;
            }
        }

        [SerializeField] private List<ResourceTypeHolder> items = new List<ResourceTypeHolder>();
        public Action<EResourceTypes, ResourceTypeHolder> OnStorageChange;

        public List<ResourceTypeHolder> Items => items;


        public ResourceTypeHolder GetStorage(ItemObject item)
        {
            return Items.Find(x => x.ResourcesType == item.Type);
        }
        
        
        public List<ResourceTypeHolder> GetStorage(EResourceTypes name)
        {
            return Items.FindAll(x => x.ResourcesType == name);
        }
        
        public ResourceTypeHolder GetStorage(EResourceTypes name, bool single)
        {
            return Items.Find(x => x.ResourcesType == name);
        }


        
        public bool IsEmptyStorage(ItemObject item, int value)
        {
            var storage = GetStorage(item);
            if (storage == null) return false;

            return storage.IsCanFitItem(item, value);
        }

        public int GetResourceByType(EResourceTypes type)
        {
            var item = Items.Find(x => x.ResourcesType == type);
            if (item != null)
            {
                return item.Count;
            }

            return 0;
        }

        public bool AddToStorage(ItemObject resourceDataItem, int resourceDataValue)
        {
            var storage = GetStorage(resourceDataItem);
            if (storage != null)
            {
                storage.Add(resourceDataItem, resourceDataValue);

                OnStorageChange?.Invoke(resourceDataItem.Type, storage);
                return true;
            }

            return false;
        }

        public bool HaveItem(ItemObject item)
        {
            var storage = GetStorage(item.Type, true);
            if (storage == null) return false;
            var itemInStorage = storage.ItemObjects.Find(x => x.Item.ID == item.ID);
            if (itemInStorage == null) return false;
            if (itemInStorage.Count == 0) return false;
            
            return true;
        }

        public ItemObject RemoveFromStorage(EResourceTypes type)
        {
            var storage = GetStorage(type, true);
            var remove = storage.RemoveItem();

            if (remove != null)
            {
                OnStorageChange?.Invoke(remove.Type, storage);
            }
            return remove;
        }

        public bool RemoveFromStorage(ItemObject item)
        {
            var storage = GetStorage(item.Type, true);
            var remove = storage.ItemObjects.Find(x => x.Item.ID == item.ID);

            if (remove != null)
            {
                storage.RemoveItem(item);
                OnStorageChange?.Invoke(item.Type, storage);
                return true;
            }

            return false;
        }

        public void LoadStorage(SaveDataObject.RaftsData.RaftStorage data, GameDataObject gameData)
        {
            for (int i = 0; i < data.StoragesData.Count; i++)
            {
                var storage = items.Find(x => x.ResourcesType == data.StoragesData[i].ResourceType);

                if (storage != null)
                {
                    for (int j = 0; j < data.StoragesData[i].ItemList.Count; j++)
                    {
                        items[i].Add(gameData.GetItem(data.StoragesData[i].ItemList[j].ItemID), data.StoragesData[i].ItemList[j].Count);
                    }
                    OnStorageChange?.Invoke(storage.ResourcesType, storage);
                }
            }
            
        }
    }
}
