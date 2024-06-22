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
        Other
    }

    public class ResourcesService : MonoBehaviour
    {
        [SerializeField] private List<RaftStorage.StorageItem> allItemsList = new List<RaftStorage.StorageItem>(20);

        private RaftBuildService raftBuildService;
        private CharacterService characterService;

        public event Action OnChangeResources;
        public List<RaftStorage.StorageItem> AllItemsList => allItemsList;

        [Inject]
        private void Construct(RaftBuildService raftBuildService, GameDataObject gameData, CharacterService characterService)
        {
            this.characterService = characterService;
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
                    var itemInArray = allItemsList.Find(x => x.Item.ID == raftStorage.Items[i].Item.ID);
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
            if (itemObject == null) return;

            var storage = raftBuildService.Storages.Find(x => x.HaveItem(itemObject));

            if (storage != null)
            {
                storage.RemoveFromStorage(itemObject);
            }
        }

        public bool GetGlobalEmptySpace(RaftStorage.StorageItem storageItem)
        {
            return GetEmptySpace() >= storageItem.Count;
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
            for (int i = 0; i < storageItem.Count; i++)
            {
                RemoveItemFromAnyRaft(storageItem.Item);
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
            var item = allItemsList.Find(x => x.Item == sellItem.Item);

            if (item != null)
            {
                return item.Count >= sellItem.Count;
            }

            return false;
        }

        public int CalculateWeaponsCount(ItemObject item)
        {
            int count = 0;
            foreach (var spawned in characterService.SpawnedCharacters)
            {
                if (spawned.AppearanceDataManager.WeaponItem != null)
                {
                    if (item == spawned.AppearanceDataManager.WeaponItem)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void RemoveWeapon(ItemObject item)
        {
            foreach (var spawned in characterService.SpawnedCharacters)
            {
                if (spawned.AppearanceDataManager.WeaponItem != null)
                {
                    if (item == spawned.AppearanceDataManager.WeaponItem)
                    {
                        spawned.Character.Equipment.SetEquipment(null, EEquipmentType.Weapon);
                        break;
                    }
                }
            }
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
                if (allItemsList.Find(x => x.Item == currentCraftIngredient.ResourceName).Count < currentCraftIngredient.Count)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
