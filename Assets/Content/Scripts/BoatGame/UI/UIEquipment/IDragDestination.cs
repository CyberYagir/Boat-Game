using Content.Scripts.ItemsSystem;
using UnityEngine.EventSystems;

namespace Content.Scripts.BoatGame.UI.UIEquipment
{
    public interface IDragDestination
    {
        public bool IsCanPlaceInside { get; }
        bool ChangeItem(ItemObject item);
        void OnBeginDrag(PointerEventData eventData);
        void OnDragManagerDragEnd();
        void OnDragManagerDragStart();
    }
}