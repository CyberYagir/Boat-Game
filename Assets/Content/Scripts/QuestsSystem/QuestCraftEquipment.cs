using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.QuestsSystem
{
    public class QuestCraftEquipment : QuestBase
    {
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnCraftingEnd += CheckCrafting;
        }
        
        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnCraftingEnd -= CheckCrafting;
        }
        private void CheckCrafting(ItemObject item, int value)
        {
            if (item.ItemType == EItemType.Armor)
            {
                AddValue(value);
            }
        }
    }
}