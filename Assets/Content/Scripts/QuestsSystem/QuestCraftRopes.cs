using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using UnityEngine;

namespace Content.Scripts.QuestsSystem
{
    public class QuestCraftRopes : QuestBase
    {
        ItemObject ropeItem;
        public override void InitQuest(QuestsEventBus eventBus, QuestDataObject questDataObject, int value)
        {
            base.InitQuest(eventBus, questDataObject, value);
            eventBus.OnCraftingEnd += CheckCrafting;
            ropeItem = questDataObject.GetDataByKey("Item") as ItemObject;
        }
        
        public override void ClaimAndDispose()
        {
            base.ClaimAndDispose();
            eventBus.OnCraftingEnd -= CheckCrafting;
        }
        private void CheckCrafting(ItemObject item, int value)
        {
            if (ropeItem == item)
            {
                AddValue(value);
            }
        }
    }
}