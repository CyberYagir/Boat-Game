using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIPotionItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text counter;
        private RaftStorage.StorageItem item;
        private UIPotionsList window;

        public void Init(RaftStorage.StorageItem item, UIPotionsList uiPotionsList)
        {
            window = uiPotionsList;
            this.item = item;

            image.sprite = item.Item.ItemIcon;
            if (item.Count > 1)
            {
                counter.text = "x" + item.Count;
            }
            else
            {
                counter.text = string.Empty;
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            window.StartDrag(item);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            window.AddEffectToSelected(item.Item);
        }
    }
}
