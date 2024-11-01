using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftsItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private UICraftSubItem subItem;
        [SerializeField] protected UICustomButton button;
        [SerializeField] private UITooltip tooltip;
        [SerializeField] private GameObject pinButton;
        
        protected List<UICraftSubItem> subItems = new List<UICraftSubItem>(10);
        protected IResourcesService resourcesService;
        protected CraftObject item;
        protected UIService uiService;
        protected IRaftBuildService raftBuildService;
        private SaveDataObject saveDataObject;

        public CraftObject Item => item;


        public virtual void Init(CraftObject item, IResourcesService resourcesService, UIService uiService, IRaftBuildService raftBuildService, SaveDataObject saveDataObject)
        {
            this.saveDataObject = saveDataObject;
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

            saveDataObject.Global.CraftPin.OnCraftPinChanged.AddListener(delegate(string n) { UpdatePin(); });
            
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
            pinButton.gameObject.SetActive(saveDataObject.Global.CraftPin.CraftID == item.Uid);
            UpdatePin();
            return canCraft;
        }

        private bool isOver;
        private void UpdatePin()
        {
            var image = pinButton.GetComponent<Image>();
            image.DOKill();
            if (saveDataObject.Global.CraftPin.CraftID != item.Uid)
            {
                image.DOColor(Color.white, 0.25f);
                if (isOver)
                {
                    if (!pinButton.gameObject.activeSelf)
                    {
                        image.SetAlpha(0);
                    }
                    pinButton.gameObject.SetActive(true);
                    image.DOFade(1, 0.25f);
                }
                else
                {
                    image.DOFade(0, 0.25f).onComplete += delegate
                    {
                        pinButton.gameObject.SetActive(false);
                    };
                }
            }
            else
            {
                image.DOColor(Color.red, 0.25f);
            }
        }

        public virtual void Build()
        {
            raftBuildService.SetTargetCraft(Item);
            uiService.ChangeGameStateToBuild();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOver = true;
            UpdatePin();
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOver = false;
            UpdatePin();
        }

        public void PinCraft()
        {
            if (saveDataObject.Global.CraftPin.CraftID != item.Uid)
            {
                saveDataObject.Global.CraftPin.ChangeCraftID(item);
            }
            else
            {
                saveDataObject.Global.CraftPin.ChangeCraftID(null);
            }
        }
    }
}
