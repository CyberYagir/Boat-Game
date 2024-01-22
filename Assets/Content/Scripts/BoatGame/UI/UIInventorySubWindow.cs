using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIInventorySubWindow : MonoBehaviour
    {
        [System.Serializable]
        public class Dragger
        {
            [SerializeField] private Image image;
            [SerializeField] private Transform item;

            private bool isDragged = false;
            private UIInventoryItem uiInventoryItem;
            public bool IsOnDrag => isDragged;

            public void Init(ItemObject obj, UIInventoryItem uiInventoryItem)
            {
                this.uiInventoryItem = uiInventoryItem;
                
                
                item.gameObject.SetActive(true);
                item.transform.localScale = Vector3.zero;
                item.transform.position = Input.mousePosition;
                item.transform.DOScale(Vector3.one, 0.25f);

                uiInventoryItem.SetState(false);
                
                image.sprite = obj.ItemIcon;
                isDragged = true;
            }
            public void MoveTo()
            {
                if (!isDragged) return;
                item.transform.position = Vector3.Lerp(item.transform.position, Input.mousePosition, 10 * Time.unscaledDeltaTime);
            }


            public void Drop()
            {
                if (isDragged)
                {
                    item.transform.DOScale(Vector3.zero, 0.25f).onComplete += () =>
                    {
                        Disable();
                    };
                }
            }

            public void Disable()
            {
                item.gameObject.SetActive(false);
                isDragged = false;
            }
        }
        
        
        [SerializeField] private UIInventoryItem item;
        [SerializeField] private Dragger dragger;
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private RaftBuildService raftBuildService;

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

        public void StartDrag(UIInventoryItem uiInventoryItem)
        {
            if (!dragger.IsOnDrag)
            {
                dragger.Init(uiInventoryItem.Item, uiInventoryItem);
            }
        }
    }
}
