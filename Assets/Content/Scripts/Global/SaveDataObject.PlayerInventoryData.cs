using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class PlayerInventoryData
        {
            [SerializeField] private RaftsData.RaftStorageData playerStorage = new RaftsData.RaftStorageData(String.Empty, new List<RaftsData.RaftStorageData.StorageItemData>());

            [NonSerialized] public UnityEvent OnChangePlayerStorage = new UnityEvent();
            
            public List<RaftsData.RaftStorageData.StorageItemData> PlayerStorageItems => playerStorage.StoragesData;

            public void AddItemsToPlayerStorage(List<RaftStorage.StorageItem> items)
            {
                playerStorage.AddToStorage(items);
                hasChanged = true;
                OnChangePlayerStorage?.Invoke();
            }

            public void SetItemsToPlayerStorage(List<RaftsData.RaftStorageData.StorageItemData> items)
            {
                playerStorage = new RaftsData.RaftStorageData(string.Empty, items);
                hasChanged = true;
                OnChangePlayerStorage?.Invoke();
            }


            private bool hasChanged = true;
            private List<RaftStorage.StorageItem> tmpStorage = new List<RaftStorage.StorageItem>(10);
            
            public List<RaftStorage.StorageItem> GetStorage(GameDataObject gameDataObject)
            {
                if (!hasChanged) return tmpStorage;
                tmpStorage.Clear();

                for (int i = 0; i < playerStorage.StoragesData.Count; i++)
                {
                    var data = playerStorage.StoragesData[i];
                    var item = gameDataObject.GetItem(data.ItemID);
                    if (item)
                    {
                        tmpStorage.Add(new RaftStorage.StorageItem(item, data.Count));
                    }
                }
                hasChanged = false;
                return tmpStorage;
            }

            public bool RemoveItem(RaftStorage.StorageItem item)
            {
                hasChanged = true;
                var state = playerStorage.RemoveFromStorage(item);
                OnChangePlayerStorage?.Invoke();
                return state;
            }
        }
    }
}