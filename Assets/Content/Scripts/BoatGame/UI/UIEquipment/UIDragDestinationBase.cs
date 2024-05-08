using System;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public abstract class UIDragDestinationBase<T> : MonoBehaviour, IBeginDragHandler, IDragDestination, IDragHandler
    {
        [SerializeField] protected Image image;
        [SerializeField] protected Image background;
        [SerializeField, ReadOnly] protected ItemObject item;
        [SerializeField] protected T type;

        protected DragAreaWindow dragAreaWindow;

        public virtual bool ChangeItem(ItemObject item)
        {
            return false;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                OnDragStart();
                dragAreaWindow.DragManager.DragStart += OnDragManagerDragStart;
                dragAreaWindow.DragManager.DragEnd += OnDragManagerDragEnd;
                dragAreaWindow.StartDrag(item, gameObject, Dragger.EDragType.ToInventory);
            }
        }

        public virtual void OnDragStart()
        {
            
        }

        public void OnDragManagerDragEnd()
        {
            image.DOFade(1, 0.2f);
            
            dragAreaWindow.DragManager.DragStart -= OnDragManagerDragStart;
            dragAreaWindow.DragManager.DragEnd -= OnDragManagerDragEnd;
        }

        public void OnDragManagerDragStart()
        {
            image.DOFade(0.2f, 0.2f);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // use to scroll view
        }
    }
}
