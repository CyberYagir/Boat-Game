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
        private IResourcesService resourceService;
        private IRaftBuildService raftBuildService;
        private GameStateService gameStateService;

        public Furnace TargetFurnace => targetFurnace;

        public void Init(
            SelectionService selectionService,
            IRaftBuildService raftBuildService,
            TickService tickService,
            IResourcesService resourceService,
            GameStateService gameStateService
        )
        {
            this.gameStateService = gameStateService;
            this.raftBuildService = raftBuildService;
            this.resourceService = resourceService;
            this.tickService = tickService;
            this.selectionService = selectionService;
            inventorySubWindow.Init(raftBuildService, this, resourceService);
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
            if (gameStateService.GameState != GameStateService.EGameState.Normal) return;
            
            
            base.ShowWindow();

            targetFurnace = selectionService.SelectedObject.Transform.GetComponent<Furnace>();
            Redraw();
            
            tickService.OnTick += OnTick;
            
            inventorySubWindow.Redraw();
        }

        private void OnTick(float obj)
        {
            if (TargetFurnace != null){
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
            slotsMap[EFurnaceSlotsType.Fuel].Init(TargetFurnace.FuelItem, this, inventorySubWindow);
            slotsMap[EFurnaceSlotsType.Smelt].Init(TargetFurnace.SmeltedItem, this, inventorySubWindow);
            slotsMap[EFurnaceSlotsType.Result].Init(TargetFurnace.ResultItem, this, inventorySubWindow);

            fireImage.fillAmount = TargetFurnace.FuelPercent;
            progressImage.fillAmount = TargetFurnace.ProgressPercent;
        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public bool SetItem(RaftStorage.StorageItem storageItem, EFurnaceSlotsType eFurnaceSlotsType, bool withResult = false)
        {
            switch (eFurnaceSlotsType)
            {
                case EFurnaceSlotsType.Fuel:

                    return ChangeItemInSlot(storageItem, TargetFurnace.FuelItem, delegate
                    {
                        TargetFurnace.SetFuel(storageItem);
                    });
                
                case EFurnaceSlotsType.Smelt:
                    return ChangeItemInSlot(storageItem, TargetFurnace.SmeltedItem, delegate
                    {
                        TargetFurnace.SetSmelt(storageItem);
                    });
                case EFurnaceSlotsType.Result:
                    if (withResult || storageItem.Item == TargetFurnace.ResultItem.Item)
                    {
                        return ChangeItemInSlot(storageItem, TargetFurnace.ResultItem, delegate
                        {
                            TargetFurnace.SetResult(storageItem);
                        });
                    }
                    return false;
            }

            return false;
        }

        private bool ChangeItemInSlot(RaftStorage.StorageItem storageItem, RaftStorage.StorageItem slot, Action SetItem)
        {
            if (slot.Item == null || slot.Count == 0)
            {
                SetItem?.Invoke();
                Redraw();
                return true;
            }
            else
            {
                if (storageItem.Item != slot.Item)
                {
                    Debug.LogError("swap?");
                    if (resourceService.TrySwapItemsWithDrop(storageItem, slot))
                    {
                        SetItem?.Invoke();
                        Redraw();
                        return true;
                    }
                }
                else
                {
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
                    RemoveFromSlot(TargetFurnace.FuelItem, dstType, i);
                    break;
                case EFurnaceSlotsType.Smelt:
                    RemoveFromSlot(TargetFurnace.SmeltedItem, dstType, i);
                    break;
                case EFurnaceSlotsType.Result:
                    RemoveFromSlot(TargetFurnace.ResultItem, dstType, i);
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
