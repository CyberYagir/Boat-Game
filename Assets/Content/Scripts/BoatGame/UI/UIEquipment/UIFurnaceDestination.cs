using System;
using Content.Scripts.ItemsSystem;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public class UIFurnaceDestination : UIDragDestinationBase<EFurnaceSlotsType>
    {
        private UIFurnaceInventorySubWindow furnaceWindow;

        public void Init(UIFurnaceInventorySubWindow dragAreaWindow)
        {
            furnaceWindow = dragAreaWindow;
            this.dragAreaWindow = dragAreaWindow;
        }

        public override bool ChangeItem(ItemObject item)
        {
            if (IsCanPlace(item))
            {
                print("can place");
            }

            return true;
        }

        public bool IsCanPlace(ItemObject item)
        {
            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanSmelt) && type == EFurnaceSlotsType.Smelt) return true;
            if (item.FurnaceData.FurnaceFlags.HasFlag(EItemFurnaceType.CanFuel) && type == EFurnaceSlotsType.Fuel) return true;

            return false;
        }
    }
}