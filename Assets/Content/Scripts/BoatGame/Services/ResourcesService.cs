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
        public Action<EResourceTypes, RaftStorage.ResourceTypeHolder> OnChangeResources;
        private RaftBuildService raftBuildService;
        private List<RaftStorage.ResourceTypeHolder> allItemsList = new List<RaftStorage.ResourceTypeHolder>(20);
        private GameDataObject gameData;
        public List<RaftStorage.ResourceTypeHolder> AllItemsList => allItemsList;

        [Inject]
        private void Construct(RaftBuildService raftBuildService, GameDataObject gameData)
        {
            this.gameData = gameData;
            this.raftBuildService = raftBuildService;
            foreach (var spawnedRaft in raftBuildService.Storages)
            {
                spawnedRaft.OnStorageChange -= OnAnyStorageChange;
                spawnedRaft.OnStorageChange += OnAnyStorageChange;
            }  
            print("execute " + transform.name);
        }

        private void OnAnyStorageChange(EResourceTypes it, RaftStorage.ResourceTypeHolder data)
        {
            var type = it;
            var newData = GetResourceData(type);

            OnChangeResources?.Invoke(type, newData);
        }

        public RaftStorage.ResourceTypeHolder GetResourceData(EResourceTypes name)
        {
            var holder = new RaftStorage.ResourceTypeHolder(name, int.MaxValue);
            
            int count = 0;
            int max = 0;
            
            for (int i = 0; i < raftBuildService.Storages.Count; i++)
            {
                var storages = raftBuildService.Storages[i].GetStorage(name);

                for (int j = 0; j < storages.Count; j++)
                {
                    for (int k = 0; k < storages[j].ItemObjects.Count; k++)
                    {
                        holder.Add(storages[j].ItemObjects[k].Item, storages[j].ItemObjects[k].Count);
                        
                        count += storages[j].Count;
                    }
                    max += storages[j].MaxCount;
                }
            }

            holder.SetMaxCount(max);

            return holder;
        }

        
        public List<RaftStorage.ResourceTypeHolder> PlayerItemsList()
        {
            AllItemsList.Clear();
            foreach (var raftStorage in raftBuildService.Storages)
            {
                AllItemsList.AddRange(raftStorage.Items);
            }

            return AllItemsList;
        }
    }
}
