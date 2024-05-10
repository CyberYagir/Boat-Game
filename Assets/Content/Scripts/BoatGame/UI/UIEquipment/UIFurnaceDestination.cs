using System;
using Content.Scripts.ItemsSystem;

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
            if (IsCanPlace(item))
            {
                return uiFurnaceWindow.SetItem(new RaftStorage.StorageItem(item, dragAreaWindow.DragManager.ItemsInStack), type);
            }

            return true;
        }

        public override void OnDragStart()
        {
            uiFurnaceWindow.RemoveFromSlot(-1, Type);
        }

        public bool IsCanPlace(ItemObject item)
        {
            if (dragAreaWindow.DragManager.DragType == Dragger.EDragType.ToInventory)
            {
                var storageItem = new RaftStorage.StorageItem(dragAreaWindow.DragManager.DraggedItem, dragAreaWindow.DragManager.ItemsInStack);
                if (item == null)
                {
                    if (uiFurnaceWindow.AddToInventory(storageItem))
                    {
                        uiFurnaceWindow.SetItem(storageItem, type);
                    }

                    return false;
                }
            }

            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt) && Type == EFurnaceSlotsType.Smelt) return true;
            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) && Type == EFurnaceSlotsType.Fuel) return true;

            return false;
        }


    }
}