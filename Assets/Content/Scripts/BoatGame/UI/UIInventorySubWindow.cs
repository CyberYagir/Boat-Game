using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventorySubWindow : MonoBehaviour
    {
        [System.Serializable]
        public class Dragger
        {
            public enum EDragType
            {
                ToEquipment,
                FromEquipment
            }
            
            [SerializeField] private Image image;
            [SerializeField] private Transform item;
            [SerializeField] private GraphicRaycaster raycaster;

            public Action DragStart;
            public Action DragEnd;
            
            private EDragType dragType;
            
            private bool isDragged = false;
            private ItemObject itemObject;
            public bool IsOnDrag => isDragged;

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
                        if (type == EDragType.ToEquipment)
                        {
                            var equipment = raycastResult.gameObject.GetComponent<UIEquipmentBase>();
                            if (equipment != null)
                            {
                                equipment.ChangeItem(itemObject);
                                break;
                            }
                        }
                        else
                        {
                            var inventory = raycastResult.gameObject.GetComponent<UIInventorySubWindow>();
                            if (inventory != null)
                            {
                                var equipment = sender.GetComponent<UIEquipmentBase>();
                                if (equipment)
                                {
                                    equipment.ChangeItem(null);
                                }

                                break;
                            }
                        }
                    }

                    DragEnd?.Invoke();
                    item.transform.DOScale(Vector3.zero, 0.25f).onComplete += () =>
                    {
                        Disable();
                    };
                    isDragged = false;

                }
            }

            public void Disable()
            {
                item.gameObject.SetActive(false);
                isDragged = false;
                DragEnd = null;
                DragStart = null;
            }
        }
        
        
        [SerializeField] private UIInventoryItem item;
        [SerializeField] private Dragger dragger;
        
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;

        public Dragger DragManager => dragger;

        public void Init(GameDataObject gameData, SelectionService selectionService, RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            Redraw();
        }

        private void OnDisable()
        {
            dragger.Disable();
        }

        public void Redraw()
        {
            for (int i = 0; i < items.Count; i++)
            {
                Destroy(items[i].gameObject);
            }

            items.Clear();
            item.gameObject.SetActive(true);
            foreach (var raftStorage in raftBuildService.Storages)
            {
                var other = raftStorage.GetStorage(EResourceTypes.Other, true);

                if (other != null)
                {
                    foreach (var otherItemObject in other.ItemObjects)
                    {
                        for (int i = 0; i < otherItemObject.Count; i++)
                        {
                            Instantiate(item, item.transform.parent)
                                .With(x => x.Init(otherItemObject.Item, this))
                                .With(x => items.Add(x));
                        }
                    }
                }
            }
            item.gameObject.SetActive(false);
        }


        private void LateUpdate()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                dragger.MoveTo();
            }
            else
            {
                dragger.Drop();
            }
        }

        public void StartDrag(ItemObject item, GameObject sender, Dragger.EDragType type)
        {
            if (!dragger.IsOnDrag)
            {
                dragger.Init(item, sender, type);
            }
        }
    }
}
