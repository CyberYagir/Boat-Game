using System;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIChestItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        private RaftStorage targetStorage;
        private ItemObject targetItem;
        private int value;
        private Action<ItemObject, bool> clickAction;

        public int Value => value;

        public ItemObject TargetItem => targetItem;

        public void Init(RaftStorage targetStorage, ItemObject targetItem, int value)
        {
            this.value = value;
            this.targetItem = targetItem;
            this.targetStorage = targetStorage;
            
            
            image.sprite = targetItem.ItemIcon;
            text.text = "x" + value;
        }


        public void DropItem()
        {
            targetStorage.RemoveFromStorage(targetItem);
        }


        public void BindButton(Action<ItemObject, bool> getItemFromStorage)
        {
            clickAction = getItemFromStorage;
            var btn = GetComponent<Button>();
            btn.onClick = new Button.ButtonClickedEvent();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                clickAction?.Invoke(targetItem, false);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                clickAction?.Invoke(targetItem, true);
            }
        }
    }
}
