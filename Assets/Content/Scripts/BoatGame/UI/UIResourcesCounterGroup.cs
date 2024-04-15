using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIResourcesCounterGroup : MonoBehaviour
    {
        [SerializeField] private EResourceTypes resourceType;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text header;
        [SerializeField] private RectTransform arrow;
        [Space]
        [SerializeField] private UIResourcesCounterItem itemPrefab;


        private List<UIResourcesCounterItem> spawnedItems = new List<UIResourcesCounterItem>(10);
        private bool isOpened;
        private UIResourcesCounter counterManager;

        public EResourceTypes ResourceType => resourceType;

        public void Init(UIResourcesCounter counterManager, GameDataObject gameData, EResourceTypes type)
        {
            this.counterManager = counterManager;
            isOpened = true;
            resourceType = type;
            icon.sprite = gameData.GetResourceIcon(type);
            header.text = type.ToString();
            arrow.transform.SetZEulerAngles(-90);
        }

        public void DrawList(List<RaftStorage.StorageItem> items)
        {
            gameObject.SetActive(items.Count > 0);
            
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].gameObject.SetActive(false);
            }
            
            if (!isOpened)
            {
                return;
            }

            for (int i = spawnedItems.Count; i < items.Count; i++)
            {
                SpawnItem();
            }

            itemPrefab.gameObject.SetActive(true);
            for (int i = 0; i < items.Count; i++)
            {
                spawnedItems[i].gameObject.SetActive(i < items.Count);
                spawnedItems[i].Init(items[i].Item.ItemIcon, items[i].Count);
            }
            itemPrefab.gameObject.SetActive(false);
        }

        private UIResourcesCounterItem SpawnItem()
        {
            return Instantiate(itemPrefab, itemPrefab.transform.parent)
                .With(x => spawnedItems.Add(x));
        }

        public void OnClick()
        {
            Switch();
        }

        private void Switch()
        {
            isOpened = !isOpened;
            counterManager.UpdateCounter();
            AnimatArrow();
        }

        private void AnimatArrow()
        {
            arrow.DOKill();
            arrow.DORotate(Vector3.forward * (isOpened ? -90f : 0f), 0.2f);
        }
    }
}
