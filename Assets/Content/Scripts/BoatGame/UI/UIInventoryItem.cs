using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Content.Scripts.BoatGame.UI.UIInventorySubWindow.Dragger;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventoryItem : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private ItemObject item;
        
        
        private UIInventorySubWindow window;
        
        public ItemObject Item => item;


        public void Init(ItemObject item, UIInventorySubWindow window)
        {
            this.window = window;
            this.item = item;
            image.sprite = item.ItemIcon;
            text.text = item.ItemName;
        }

        public void OnDrag(PointerEventData eventData)
        {
            window.DragManager.DragStart += delegate
            {
                SetState(true);
            };
            window.DragManager.DragEnd += delegate
            {
                SetState(false);
            };
            window.StartDrag(item, gameObject, EDragType.ToEquipment);
        }

        public void SetState(bool state)
        {
            image.DOFade(state ? 0.2f : 1f, 0.2f);
            text.DOFade(state ? 0.2f : 1f, 0.2f);
            
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }
}
