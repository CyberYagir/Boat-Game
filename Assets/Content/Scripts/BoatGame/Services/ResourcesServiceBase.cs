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
    }
}