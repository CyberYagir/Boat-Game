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
