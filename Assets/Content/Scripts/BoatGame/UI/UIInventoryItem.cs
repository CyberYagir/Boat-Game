using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private ItemObject item;
        
        
        private DragAreaWindow window;
        
        public ItemObject Item => item;


        public void Init(ItemObject item, DragAreaWindow window)
        {
            this.window = window;
            this.item = item;
            image.sprite = item.ItemIcon;
            text.text = item.ItemName;
        }
        
        public void SetValue(int stackedValue)
        {
            text.text += $" x" + stackedValue;  
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            window.DragManager.DragStart += OnDragStartEvent;
            window.DragManager.DragEnd += OnDragStopEvent;
            window.StartDrag(item, gameObject, Dragger.EDragType.ToDestination);
        }
        
        private void OnDragStopEvent()
        {
            SetState(false);
        }

        private void OnDragStartEvent()
        {
            SetState(true);
        }

        public void SetState(bool state)
        {
            image.DOFade(state ? 0.2f : 1f, 0.2f);
            text.DOFade(state ? 0.2f : 1f, 0.2f);

            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);


            if (state == false)
            {
                DisableItem();
            }
        }


        public void DisableItem()
        {
            window.DragManager.DragStart -= OnDragStartEvent;
            window.DragManager.DragEnd -= OnDragStopEvent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // use to scroll view
        }


    }
}
