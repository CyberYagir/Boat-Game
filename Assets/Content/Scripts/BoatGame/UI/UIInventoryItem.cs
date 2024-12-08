using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private ItemObject item;
        [SerializeField] private GameObject inspectButton;
        
        private DragAreaWindow window;
        private IUIService uiService;

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
            text.text = $"{item.ItemName} x" + stackedValue;  
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            window.DragManager.DragStart += OnDragStartEvent;
            window.DragManager.DragEnd += OnDragStopEvent;
            window.StartDrag(item, 1, window.gameObject);
        }
        
        private void OnDragStopEvent()
        {
            SetState(false);
        }

        private void OnDragStartEvent()
        {
            SetState(true);
        }

        public void SetState(bool state, bool withPunch = true)
        {
            image.DOFade(state ? 0.2f : 1f, 0.2f);
            text.DOFade(state ? 0.2f : 1f, 0.2f);

            if (withPunch)
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
                transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }

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


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (inspectButton != null)
            {
                inspectButton.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (inspectButton != null)
            {
                inspectButton.SetActive(false);
            }
        }

        public void InitInfo(IUIService uiService)
        {
            if (inspectButton != null)
            {
                this.uiService = uiService;
                var button = inspectButton.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate { uiService.PreviewItem(item); });
            }
        }
    }
}
