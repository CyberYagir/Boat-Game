using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIResourcesCounterItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject trashIcon;
        private UIResourcesCounter mainCounter;
        private ItemObject itemObject;

        public void Init(int value, ItemObject itemObject, UIResourcesCounter mainCounter)
        {
            this.itemObject = itemObject;
            this.mainCounter = mainCounter;
            image.sprite = itemObject.ItemIcon;
            text.text = value.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mainCounter.RemoveItem(itemObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            trashIcon.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            trashIcon.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            trashIcon.gameObject.SetActive(false);
        }
    }
}
