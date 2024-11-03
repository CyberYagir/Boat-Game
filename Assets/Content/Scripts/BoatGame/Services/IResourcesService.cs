using System;
using System.Collections.Generic;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;

namespace Content.Scripts.BoatGame.Services
{
    public interface IResourcesService
    {
        public event Action<ItemObject, int> OnAddStorageItem; 
        public event Action<ItemObject> OnAddItemToRaft; 
        public Dictionary<ItemObject, int> AllItemsList { get; }
        event Action OnChangeResources;
        void PlayerItemsList();
        void RemoveItemFromAnyRaft(ItemObject itemObject);
        List<RaftStorage.StorageItem> GetItemsByType(EResourceTypes Type);
        List<RaftStorage.StorageItem> GetItemsByTypes(List<EResourceTypes> types);
        void AddItemsToAnyRafts(RaftStorage.StorageItem oldItem, bool spawnPopup = true);

        bool AddToAnyStorage(ItemObject item);

        bool TrySwapItems(RaftStorage.StorageItem newItem, RaftStorage.StorageItem oldItem);

        bool GetGlobalEmptySpace(RaftStorage.StorageItem storageItem, int offcet = 0);

        int GetEmptySpace();
        int GetItemsValue(ItemObject resourceName);
        bool TrySwapItemsWithDrop(RaftStorage.StorageItem storageItem, RaftStorage.StorageItem slot);
        bool IsCanTradeItem(RaftStorage.StorageItem getStorageItem, RaftStorage.StorageItem storageItem);
        bool IsHaveItem(RaftStorage.StorageItem storageItem);
        bool IsHaveItem(ItemObject configDataHealSlaveItem, int i);
        void RemoveItemsFromAnyRaft(RaftStorage.StorageItem sellItem);
        List<RaftStorage> FindEmptyStorages(ItemObject itemObject, int value);
        RaftStorage FindStorageByResource(EResourceTypes type);
        bool HaveMaterialsForCrafting(List<CraftObject.CraftItem> currentCraftIngredients);
    }
}