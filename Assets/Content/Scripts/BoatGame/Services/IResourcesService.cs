﻿using System;
using System.Collections.Generic;
using Content.Scripts.ItemsSystem;

namespace Content.Scripts.BoatGame.Services
{
    public interface IResourcesService
    {
        public Dictionary<ItemObject, int> AllItemsList { get; }
        event Action OnChangeResources;
        void PlayerItemsList();
        void RemoveItemFromAnyRaft(ItemObject itemObject);
        List<RaftStorage.StorageItem> GetItemsByType(EResourceTypes Type);
        void AddItemsToAnyRafts(RaftStorage.StorageItem oldItem, bool spawnPopup = true);

        bool AddToAnyStorage(ItemObject item);

        bool TrySwapItems(RaftStorage.StorageItem newItem, RaftStorage.StorageItem oldItem);
    }
}