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
        
        private Dictionary<ItemObject, int> stackedItems = new Dictionary<ItemObject, int>(50);
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;
        private UIFurnaceWindow furnaceWindow;

        public void Init(RaftBuildService raftBuildService, UIFurnaceWindow furnaceWindow)
        {
            this.furnaceWindow = furnaceWindow;
            this.raftBuildService = raftBuildService;
            Redraw();
        }
        

        protected override void AddItemToStack()
        {
            if (DragManager.DragType == Dragger.EDragType.ToDestination)
            {
                if (stackedItems[DragManager.DraggedItem] > DragManager.ItemsInStack)
                {
                    DragManager.SetStack(DragManager.ItemsInStack + 1);
                    UpdateStackText();
                }
            }
            else if (DragManager.DragType == Dragger.EDragType.ToInventory)
            {
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
        }

        private void UpdateStackText()
        {
            if (DragManager.IsOnDrag)
            {
                if (stackedItems.ContainsKey(DragManager.DraggedItem))
                {
                    var spawnedItem = items.Find(x => x.Item == DragManager.DraggedItem);
                    spawnedItem.With(x => x.Init(DragManager.DraggedItem, this))
                        .With(x => x.SetValue(stackedItems[DragManager.DraggedItem] - DragManager.ItemsInStack));
                }
            }
        }


        public override void OnDragStarted()
        {
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
                items[i].DisableItem();
            }
            
            item.gameObject.SetActive(true);
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetItem(EResourceTypes.Eat);
                var build = raftStorage.GetItem(EResourceTypes.Build);

                foreach (var item in other)
                {
                    CalculateItemToStack(item);
                }
                
                foreach (var item in build)
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
        }

        private void CalculateItemToStack(RaftStorage.StorageItem item)
        {
            if (item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) || item.Item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt))
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
