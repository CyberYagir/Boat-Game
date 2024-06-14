using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftingTableItem : UICraftsItem
    {
        [SerializeField] private TMP_Text count;

        public override bool UpdateItem()
        {
            if (item.FinalItem.Count > 1)
            {
                count.text = "x" + item.FinalItem.Count;
            }
            else
            {
                count.text = "";
            }

            if (base.UpdateItem())
            {
                transform.SetSiblingIndex(0);
                int canHoldItems = resourcesService.GetEmptySpace();

                for (int i = 0; i < item.Ingredients.Count; i++)
                {
                    canHoldItems += item.Ingredients[i].Count;
                }


                if (canHoldItems < item.FinalItem.Count)
                {
                    button.SetTransparent();
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