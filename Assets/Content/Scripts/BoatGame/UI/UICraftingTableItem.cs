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

                var craftsOffcet = 0;
                for (int i = 0; i < item.Ingredients.Count; i++)
                {
                    if (item.Ingredients[i].ResourceName.HasSize)
                    {
                        craftsOffcet += item.Ingredients.Count;
                    }
                }
                
                if (!resourcesService.GetGlobalEmptySpace(Item.FinalItem.ToStorageItem(), craftsOffcet))
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