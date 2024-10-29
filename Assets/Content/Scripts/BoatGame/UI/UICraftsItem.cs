using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftsItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private UICraftSubItem subItem;
        [SerializeField] protected UICustomButton button;
        [SerializeField] private UITooltip tooltip;
        
        protected List<UICraftSubItem> subItems = new List<UICraftSubItem>(10);
        protected IResourcesService resourcesService;
        protected CraftObject item;
        protected UIService uiService;
        protected IRaftBuildService raftBuildService;

        public CraftObject Item => item;


        public void Init(CraftObject item, IResourcesService resourcesService, UIService uiService, IRaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            this.uiService = uiService;
            this.item = item;
            this.resourcesService = resourcesService;
            
            
            icon.sprite = item.Icon;
            text.text = item.CraftName;

            if (tooltip != null)
            {
                if (item.Tooltip)
                {
                    tooltip.Init(item.Tooltip);
                }
                else
                {
                    tooltip.gameObject.SetActive(false);
                }
            }

            subItem.gameObject.SetActive(true);
            foreach (var ing in item.Ingredients)
            {
                Instantiate(subItem, subItem.transform.parent)
                    .With(x => x.Init(ing.Count, ing.ResourceName.ItemIcon))
                    .With(x => subItems.Add(x));
            }

            subItem.gameObject.SetActive(false);
            
            
            UpdateItem();
        }

        public virtual bool UpdateItem()
        {
            bool canCraft = true;
            for (int i = 0; i < subItems.Count; i++)
            {
                int count = 0;
                
                var itemCount = resourcesService.GetItemsValue(Item.Ingredients[i].ResourceName);
                count += itemCount;

                if (count < Item.Ingredients[i].Count)
                {
                    canCraft = false;
                }
                
                subItems[i].UpdateItem(count);
            }

            button.SetInteractable(canCraft);

            return canCraft;
        }

        public virtual void Build()
        {
            raftBuildService.SetTargetCraft(Item);
            uiService.ChangeGameStateToBuild();
        }
    }
}
