using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIFurnaceWindow : AnimatedWindow
    {
        [SerializeField] private UIFurnaceInventorySubWindow inventorySubWindow;
        [SerializeField] private List<UIFurnaceDestination> slots;
        [SerializeField] private Image fireImage;
        [SerializeField] private Image progressImage;
        
        
        private Furnace targetFurnace;
        private SelectionService selectionService;
        private Dictionary<EFurnaceSlotsType, UIFurnaceDestination> slotsMap;
        private TickService tickService;
        private ResourcesService resourceService;
        private RaftBuildService raftBuildService;

        public void Init(
            SelectionService selectionService,
            RaftBuildService raftBuildService,
            TickService tickService,
            ResourcesService resourceService
        )
        {
            this.raftBuildService = raftBuildService;
            this.resourceService = resourceService;
            this.tickService = tickService;
            this.selectionService = selectionService;
            inventorySubWindow.Init(raftBuildService, this);
            slotsMap = slots.ToDictionary(x => x.Type);
            
            
            raftBuildService.OnChangeRaft += ChangeRaftsEvents;
            
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);

        }

        private void ChangeRaftsEvents()
        {
            foreach (var storage in raftBuildService.Storages)
            {
                storage.OnStorageChange -= StorageOnOnStorageChange; 
                storage.OnStorageChange += StorageOnOnStorageChange; 
            }
        }

        private void StorageOnOnStorageChange()
        {
            inventorySubWindow.Redraw();
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionFurnace>();
            furnaceAction.OnOpenWindow -= ShowWindow;
            furnaceAction.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }

        public override void ShowWindow()
        {
            base.ShowWindow();

            targetFurnace = selectionService.SelectedObject.Transform.GetComponent<Furnace>();
            Redraw();
            
            tickService.OnTick += OnTick;
            
            inventorySubWindow.Redraw();
        }

        private void OnTick(float obj)
        {
            if (targetFurnace != null){
                Redraw();
            }
        }

        public override void OnClosed()
        {
            base.OnClosed();
            tickService.OnTick -= OnTick;
        }

        private void Redraw()
        {

            slotsMap[EFurnaceSlotsType.Fuel].Init(targetFurnace.FuelItem, this, inventorySubWindow);
            slotsMap[EFurnaceSlotsType.Smelt].Init(targetFurnace.SmeltedItem, this, inventorySubWindow);
            slotsMap[EFurnaceSlotsType.Result].Init(targetFurnace.ResultItem, this, inventorySubWindow);

            fireImage.fillAmount = targetFurnace.FuelPercent;
            progressImage.fillAmount = targetFurnace.ProgressPercent;
        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public bool SetItem(RaftStorage.StorageItem storageItem, EFurnaceSlotsType eFurnaceSlotsType)
        {
            switch (eFurnaceSlotsType)
            {
                case EFurnaceSlotsType.Fuel:

                    return ChangeItemInSlot(storageItem, targetFurnace.FuelItem, delegate
                    {
                        targetFurnace.SetFuel(storageItem);
                    });
                    
                    break;
                case EFurnaceSlotsType.Smelt:
                    return ChangeItemInSlot(storageItem, targetFurnace.SmeltedItem, delegate
                    {
                        targetFurnace.SetSmelt(storageItem);
                    });
                    break;
                case EFurnaceSlotsType.Result:
                    break;
            }

            return false;
        }

        private bool ChangeItemInSlot(RaftStorage.StorageItem storageItem, RaftStorage.StorageItem slot, Action SetItem)
        {
            if (slot.Item == null || slot.Count == 0)
            {
                resourceService.RemoveItemsFromAnyRaft(storageItem);
                SetItem?.Invoke();
                Redraw();
                return true;
            }
            else
            {
                if (storageItem.Item != targetFurnace.FuelItem.Item)
                {
                    if (resourceService.TrySwapItems(storageItem, slot))
                    {
                        SetItem?.Invoke();
                        Redraw();
                        return true;
                    }
                }
                else
                {
                    resourceService.RemoveItemsFromAnyRaft(storageItem);
                    slot.Add(storageItem.Count);
                    Redraw();
                    return true;
                }
            }

            return false;
        }

        public void RemoveFromSlot(int i, EFurnaceSlotsType dstType)
        {
            switch (dstType)
            {
                case EFurnaceSlotsType.Fuel:
                    RemoveFromSlot(targetFurnace.FuelItem, dstType, i);

                    break;
                case EFurnaceSlotsType.Smelt:
                    RemoveFromSlot(targetFurnace.SmeltedItem, dstType, i);
                    break;
                case EFurnaceSlotsType.Result:
                    RemoveFromSlot(targetFurnace.ResultItem, dstType, i);
                    break;
            }
        }



        private void RemoveFromSlot(RaftStorage.StorageItem slot, EFurnaceSlotsType dstType, int i)
        {
            if (slot.Item != null && slot.Count > 0)
            {
                slotsMap[dstType].Add(i);
                slot.Add(-1);
            }
        }

        public bool AddToInventory(RaftStorage.StorageItem storageItem)
        {
            if (resourceService.GetGlobalEmptySpace(storageItem))
            {
                resourceService.AddItemsToAnyRafts(storageItem);
                return true;
            }

            return false;
        }
    }
}
