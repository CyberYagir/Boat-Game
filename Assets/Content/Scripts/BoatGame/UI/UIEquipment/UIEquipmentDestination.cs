using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public class UIEquipmentDestination : UIDragDestinationBase<EEquipmentType>
    {
        private UICharacterWindow window;
        
        public void Init(Character character, GameDataObject gameDataObject, UICharacterWindow uiCharacterWindow, UICharacterInventorySubWindow uiInventorySubWindow)
        {
            dragAreaWindow = uiInventorySubWindow;
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

            SetIconAndBackground();
        }

        public override void OnDragStart()
        {
            window.ChangeTabToInventory();
        }

        public override bool ChangeItem(ItemObject item)
        {
            image.transform.DOKill();
            var state = window.ChangeEquipment(item, type);
            
            if (item == null && state != false)
            {
                image.transform.DOScale(Vector3.zero, 0.2f);
            }
            else
            {
                image.transform.localScale = Vector3.one;
                image.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }


            if (state == false)
            {
                SetIconAndBackground();
            }

            return state;
        }

        private void SetIconAndBackground()
        {
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
        
    }
}