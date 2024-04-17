using System;
using System.Collections.Generic;
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
        Other
    }
    public class ResourcesService : MonoBehaviour
    {
        [SerializeField] private List<RaftStorage.StorageItem> allItemsList = new List<RaftStorage.StorageItem>(20);
        
        private RaftBuildService raftBuildService;
        
        public Action OnChangeResources;
        public List<RaftStorage.StorageItem> AllItemsList => allItemsList;

        [Inject]
        private void Construct(RaftBuildService raftBuildService, GameDataObject gameData)
        {
            this.raftBuildService = raftBuildService;
            foreach (var spawnedRaft in raftBuildService.Storages)
            {
                spawnedRaft.OnStorageChange -= OnAnyStorageChange;
                spawnedRaft.OnStorageChange += OnAnyStorageChange;
            }  
            print("execute " + transform.name);
        }

        private void OnAnyStorageChange()
        {
            OnChangeResources?.Invoke();
        }


        public void PlayerItemsList()
        {
            allItemsList.Clear();
            foreach (var raftStorage in raftBuildService.Storages)
            {
                for (int i = 0; i < raftStorage.Items.Count; i++)
                {
                    var itemInArray = allItemsList.Find(x=>x.Item.ID == raftStorage.Items[i].Item.ID);
                    if (itemInArray == null)
                    {
                        allItemsList.Add(new RaftStorage.StorageItem(raftStorage.Items[i].Item, raftStorage.Items[i].Count));
                    }
                    else
                    {
                        itemInArray.Add(raftStorage.Items[i].Count);
                    }
                }
            }
        }

        public void RemoveItemFromAnyRaft(ItemObject itemObject)
        {
            var storage = raftBuildService.Storages.Find(x => x.HaveItem(itemObject));
            
            if (storage != null)
            {
                storage.RemoveFromStorage(itemObject);
            }
        }
    }
}
