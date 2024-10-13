using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIFurnaceInventorySubWindow : DragAreaWindow
    {
        [SerializeField] private UIInventoryItem item;
        [SerializeField] private GameObject itemsNotFoundText;
        
        private Dictionary<ItemObject, int> stackedItems = new Dictionary<ItemObject, int>(50);
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;
        private UIFurnaceWindow furnaceWindow;
        private ResourcesService resourcesService;

        public void Init(RaftBuildService raftBuildService, UIFurnaceWindow furnaceWindow, ResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            this.furnaceWindow = furnaceWindow;
            this.raftBuildService = raftBuildService;

            DragManager.OnNotInAreaDrop += OnNotInAreaDrop;

            Redraw();
        }

        private void OnNotInAreaDrop()
        {
            AddToInventory(DragManager.DraggedItem);
        }


        protected override void AddItemToStack()
        {
            if (!DragManager.IsOnDrag) return;

            if (DragManager.GetInventoryUnderMouse() != null)
            {
                if (stackedItems.ContainsKey(DragManager.DraggedItem))
                {
                    if (stackedItems[DragManager.DraggedItem] > 0)
                    {
                        RemoveItemFromInventory();
                        DragManager.SetStack(DragManager.ItemsInStack + 1);
                        UpdateStackText();
                    }
                }

                return;
            }

            var destination = DragManager.GetDestinationUnderMouse();

            if (destination != null)
            {
                var dst = destination as UIDragDestinationBase<EFurnaceSlotsType>;
                if (dst != null)
                {
                    if (dst.Count > 0)
                    {
                        DragManager.SetStack(DragManager.ItemsInStack + 1);
                        furnaceWindow.RemoveFromSlot(-1, dst.Type);
                    }
                }
            }
        }

        private void RemoveItemFromInventory()
        {
            stackedItems[DragManager.DraggedItem]--;
            resourcesService.RemoveItemFromAnyRaft(DragManager.DraggedItem);
        }

        private void UpdateStackText()
        {
            if (DragManager.IsOnDrag)
            {
                if (stackedItems.ContainsKey(DragManager.DraggedItem))
                {
                    var spawnedItem = items.Find(x => x.Item == DragManager.DraggedItem);
                    spawnedItem.With(x => x.Init(DragManager.DraggedItem, this))
                        .With(x => x.SetValue(stackedItems[DragManager.DraggedItem]));
                }
            }
        }


        public override void OnDragStarted()
        {
            if (DragManager.GetInventoryUnderMouse() != null)
            {
                stackedItems[DragManager.DraggedItem]--;
                resourcesService.RemoveItemFromAnyRaft(DragManager.DraggedItem);
            }

            UpdateStackText();
        }

        public override void AddToInventory(ItemObject draggedItem)
        {
            base.AddToInventory(draggedItem);

            var emptySpace = resourcesService.GetEmptySpace();

            if (emptySpace >= DragManager.ItemsInStack)
            {
                if (stackedItems.ContainsKey(DragManager.DraggedItem))
                {
                    stackedItems[DragManager.DraggedItem] += DragManager.ItemsInStack;
                }

                resourcesService.AddItemsToAnyRafts(new RaftStorage.StorageItem(draggedItem, DragManager.ItemsInStack));
            }
            else
            {
                if (stackedItems.ContainsKey(DragManager.DraggedItem))
                {
                    stackedItems[DragManager.DraggedItem] += emptySpace;
                }
                resourcesService.AddItemsToAnyRafts(new RaftStorage.StorageItem(draggedItem, emptySpace));
                DragManager.SetStack(DragManager.ItemsInStack - emptySpace);
                DragManager.ReturnPartOfStack();
            }
            
            UpdateStackText();
        }

        public override void OnDragDropped()
        {
            Redraw();
        }

        public void Redraw()
        {
            stackedItems.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetState(false, false);
            }

            item.gameObject.SetActive(true);
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetItem(EResourceTypes.Eat);
                var build = raftStorage.GetItem(EResourceTypes.Build);
                var money = raftStorage.GetItem(EResourceTypes.Money);

                foreach (var item in other)
                {
                    CalculateItemToStack(item);
                }

                foreach (var item in build)
                {
                    CalculateItemToStack(item);
                }
                
                foreach (var item in money)
                {
                    CalculateItemToStack(item);
                }
            }

            int count = 0;
            foreach (var stacked in stackedItems)
            {
                if (stacked.Value > 0)
                {
                    if (count >= items.Count)
                    {
                        Instantiate(item, item.transform.parent)
                            .With(x => x.Init(stacked.Key, this))
                            .With(x => x.SetValue(stacked.Value))
                            .With(x => items.Add(x));
                    }
                    else
                    {
                        items[count].Init(stacked.Key, this);
                        items[count].SetValue(stacked.Value);
                        items[count].gameObject.SetActive(true);
                    }

                    count++;
                }
            }

            for (int i = count; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }

            UpdateStackText();

            item.gameObject.SetActive(false);
            
            itemsNotFoundText.gameObject.SetActive(count == 0);
        }

        private void CalculateItemToStack(RaftStorage.StorageItem item)
        {
            if (
                item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) ||
                item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt) ||
                item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanDisplay)
            )
            {
                if (!stackedItems.ContainsKey(item.Item))
                {

                    stackedItems.Add(item.Item, 0);
                }

                stackedItems[item.Item] += item.Count;
            }
        }
    }
}
