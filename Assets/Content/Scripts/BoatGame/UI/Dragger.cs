using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class Dragger
    {
        public enum EDragType
        {
            ToDestination,
            ToInventory
        }

        [SerializeField] private Image image;
        [SerializeField] private Transform item;
        [SerializeField] private TMP_Text stackCounter;
        [SerializeField] private GraphicRaycaster raycaster;

        public Action DragStart;
        public Action DragEnd;

        private bool isDragged = false;
        private ItemObject itemObject;
        private int itemsInStack = 1;
        
        public bool IsOnDrag => isDragged;

        public ItemObject DraggedItem => itemObject;

        public int ItemsInStack => itemsInStack;

        public void SetStack(int value)
        {
            itemsInStack = value;
        }

        public void Init(ItemObject obj, GameObject sender, EDragType type)
        {
            this.sender = sender;
            Init(obj, type);

            DragStart?.Invoke();
        }

        private void Init(ItemObject obj, EDragType type)
        {
            this.type = type;
            itemObject = obj;
            if (raycaster == null)
            {
                raycaster = this.sender.GetComponentInParent<GraphicRaycaster>();
            }

            item.gameObject.SetActive(true);
            item.transform.localScale = Vector3.zero;
            item.transform.position = Input.mousePosition;
            item.transform.DOScale(Vector3.one, 0.25f);

            image.sprite = obj.ItemIcon;
            isDragged = true;
        }

        private List<RaycastResult> result = new List<RaycastResult>(20);
        private EDragType type;
        private GameObject sender;

        public void MoveTo()
        {
            if (!isDragged) return;

            if (stackCounter != null)
            {
                stackCounter.text = ItemsInStack > 1 ? $"x{ItemsInStack}" : "";
            }
            item.transform.position = Vector3.Lerp(item.transform.position, Input.mousePosition, 10 * Time.unscaledDeltaTime);
        }


        public void Drop()
        {
            if (isDragged)
            {
                result.Clear();
                raycaster.Raycast(new PointerEventData(EventSystem.current) {position = Input.mousePosition}, result);
                foreach (var raycastResult in result)
                {
                    if (type == EDragType.ToDestination)
                    {
                        var equipment = raycastResult.gameObject.GetComponent<IDragDestination>();
                        if (equipment != null)
                        {
                            equipment.ChangeItem(DraggedItem);
                            break;
                        }
                    }
                    else
                    {
                        var inventory = raycastResult.gameObject.GetComponent<IDragDropArea>();
                        if (inventory != null)
                        {
                            var equipment = sender.GetComponent<IDragDestination>();
                            if (equipment != null)
                            {
                                equipment.ChangeItem(null);
                            }

                            break;
                        }
                    }
                }

                DragEnd?.Invoke();
                item.transform.DOScale(Vector3.zero, 0.25f).onComplete += Disable;
                ResetEvents();
            }
        }

        private void ResetEvents()
        {
            isDragged = false;
            DragEnd = null;
            DragStart = null;
            itemsInStack = 1;
        }

        public void Disable()
        {
            item.gameObject.SetActive(false);
            ResetEvents();
        }
    }
}