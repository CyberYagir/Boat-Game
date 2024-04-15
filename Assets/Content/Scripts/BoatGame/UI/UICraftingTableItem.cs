namespace Content.Scripts.BoatGame.UI
{
    public class UICraftingTableItem : UICraftsItem
    {
        public override bool UpdateItem()
        {
            if (base.UpdateItem())
            {
                int canHoldItems = 0;
                foreach (var raftStorage in raftBuildService.Storages)
                {
                    canHoldItems += raftStorage.GetEmptySlots();
                }

                if (canHoldItems < item.FinalItem.Count)
                {
                    button.interactable = false;
                }
            }

            return false;
        }

        public override void Build()
        {
            uiService.CharacterCraftItem(item);
        }
    }   
}