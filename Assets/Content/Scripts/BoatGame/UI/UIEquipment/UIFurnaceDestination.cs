using System;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public class UIFurnaceDestination : UIDragDestinationBase<EFurnaceSlotsType>
    {
        private UIFurnaceWindow uiFurnaceWindow;

        public void Init(RaftStorage.StorageItem item, UIFurnaceWindow uiFurnaceWindow, UIFurnaceInventorySubWindow dragAreaWindow)
        {
            this.uiFurnaceWindow = uiFurnaceWindow;
            this.dragAreaWindow = dragAreaWindow;

            background.gameObject.SetActive(false);


            image.gameObject.SetActive(item.Item != null && item.Count > 0);
            if (item.Item != null)
            {
                image.sprite = item.Item.ItemIcon;
                itemCounter.text = item.Count != 0 ? "x" + item.Count : "";
            }

            this.item = item.Item;
            this.count = item.Count;
        }

        public override bool ChangeItem(ItemObject item)
        {
            var isCanPlace = IsCanPlace(item);
            print(isCanPlace);
            if (isCanPlace)
            {
                if (uiFurnaceWindow.SetItem(new RaftStorage.StorageItem(item, dragAreaWindow.DragManager.ItemsInStack), type))
                {
                    return true;
                }
                else
                {
                    dragAreaWindow.AddToInventory(dragAreaWindow.DragManager.DraggedItem);
                    return false;
                }
            }

            return true;
        }

        public override void OnDragStart()
        {
            uiFurnaceWindow.RemoveFromSlot(-1, Type);
        }

        public bool IsCanPlace(ItemObject item)
        {
            var storageItem = new RaftStorage.StorageItem(dragAreaWindow.DragManager.DraggedItem, dragAreaWindow.DragManager.ItemsInStack);
            if (item == null)
            {
                var isCanAddToInventory = uiFurnaceWindow.AddToInventory(storageItem);
                if (!isCanAddToInventory)
                {
                    uiFurnaceWindow.SetItem(storageItem, type, true);
                }

                return false;
            }

            
            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt) && Type == EFurnaceSlotsType.Smelt) return true;
            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) && Type == EFurnaceSlotsType.Fuel) return true;
            if (item == uiFurnaceWindow.TargetFurnace.ResultItem.Item && Type == EFurnaceSlotsType.Result) return true;
            
            
            uiFurnaceWindow.AddToInventory(storageItem);
            
            return false;
        }


    }
}