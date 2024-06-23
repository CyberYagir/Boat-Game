using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftingTableItem : UICraftsItem
    {
        [SerializeField] private TMP_Text count;

        public override bool UpdateItem()
        {
            if (Item.FinalItem.Count > 1)
            {
                count.text = "x" + Item.FinalItem.Count;
            }
            else
            {
                count.text = "";
            }

            if (base.UpdateItem())
            {
                transform.SetSiblingIndex(0);
                int canHoldItems = resourcesService.GetEmptySpace();

                for (int i = 0; i < Item.Ingredients.Count; i++)
                {
                    canHoldItems += Item.Ingredients[i].Count;
                }


                if (canHoldItems < Item.FinalItem.Count)
                {
                    button.SetTransparent();
                }
            }

            return false;
        }

        public override void Build()
        {
            uiService.CharacterCraftItem(Item);
        }
    }   
}