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
        [SerializeField] protected Button button;
        [SerializeField] private UITooltip tooltip;
        
        protected List<UICraftSubItem> subItems = new List<UICraftSubItem>(10);
        protected ResourcesService resourcesService;
        protected CraftObject item;
        protected UIService uiService;
        protected RaftBuildService raftBuildService;


        public void Init(CraftObject item, ResourcesService resourcesService, UIService uiService, RaftBuildService raftBuildService)
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
                
                var items = resourcesService.AllItemsList.FindAll(x => x.Item.ID == item.Ingredients[i].ResourceName.ID);
                count += items.Sum(x => x.Count);

                if (count < item.Ingredients[i].Count)
                {
                    canCraft = false;
                }
                
                subItems[i].UpdateItem(count);
            }

            button.interactable = canCraft;
            button.image.DOColor(button.interactable ? new Color(0.3380358f, 1f, 0, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f), 0.2f).SetLink(button.gameObject);

            return canCraft;
        }

        public virtual void Build()
        {
            raftBuildService.SetTargetCraft(item);
            uiService.ChangeGameStateToBuild();
        }
    }
}
