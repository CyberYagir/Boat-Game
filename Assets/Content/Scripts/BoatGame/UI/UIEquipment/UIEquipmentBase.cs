using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public class UIEquipmentBase : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public enum EEquipmentType
        {
            Helmet,
            Armor,
            Weapon
        }
        
        [SerializeField] private Image image;
        [SerializeField] private Image background;
        [SerializeField, ReadOnly] private ItemObject item;
        [SerializeField] private EEquipmentType type;
        
        private UICharacterWindow window;
        private UIInventorySubWindow inventorySubWindow;


        public void Init(Character character, GameDataObject gameDataObject, UICharacterWindow uiCharacterWindow, UIInventorySubWindow uiInventorySubWindow)
        {
            inventorySubWindow = uiInventorySubWindow;
            this.window = uiCharacterWindow;

            switch (type)
            {
                case EEquipmentType.Helmet:
                    item = gameDataObject.GetItem(character.Equipment.HelmetID);
                    break;
                case EEquipmentType.Armor:
                    item = gameDataObject.GetItem(character.Equipment.ArmorID);
                    break;
                case EEquipmentType.Weapon:
                    item = gameDataObject.GetItem(character.Equipment.WeaponID);
                    break;
            }

            if (item != null)
            {
                image.transform.localScale = Vector3.one;
                image.enabled = true;
                background.enabled = false;
                image.sprite = item.ItemIcon;
            }
            else
            {
                image.enabled = false;
                background.enabled = true;
            }
        }


        public void ChangeItem(ItemObject item)
        {
            window.ChangeEquipment(item, type);
            image.transform.DOKill();
            if (item == null)
            {
                image.transform.DOScale(Vector3.zero, 0.2f);
            }
            else
            {
                image.transform.localScale = Vector3.one;
                image.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
          
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                
                Debug.LogError("Start Drag");
                window.ChangeTabToInventory();
                inventorySubWindow.DragManager.DragStart += OnDragManagerDragStart;
                inventorySubWindow.DragManager.DragEnd += OnDragManagerDragEnd;
                
                inventorySubWindow.StartDrag(item, gameObject, UIInventorySubWindow.Dragger.EDragType.FromEquipment);
            }
        }

        private void OnDragManagerDragEnd()
        {
            image.DOFade(1, 0.2f);
            
            Debug.LogError("End Drag");
            
            inventorySubWindow.DragManager.DragStart -= OnDragManagerDragStart;
            inventorySubWindow.DragManager.DragEnd -= OnDragManagerDragEnd;
        }

        private void OnDragManagerDragStart()
        {
            image.DOFade(0.2f, 0.2f);
        }
    }
}
