using System;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UISoulsCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform counterPosition;
        [SerializeField] private UIStoragesCounter storagesCounter;
        private RectTransform rectTransform;
        private Vector3 startPosition;
        
        private SaveDataObject saveDataObject;

        public void Init(SaveDataObject saveDataObject)
        {
            rectTransform = GetComponent<RectTransform>();
            startPosition = rectTransform.anchoredPosition;
            this.saveDataObject = saveDataObject;
            saveDataObject.CrossGame.OnSoulsChanged.AddListener(UpdateCounter);
            UpdateCounter();
        }

        private void Update()
        {
            if (saveDataObject.CrossGame.SoulsCount > 0)
            {
                if (storagesCounter.IsVisible)
                {
                    rectTransform.anchoredPosition = startPosition;
                }
                else
                {
                    rectTransform.anchoredPosition = counterPosition.anchoredPosition;
                }
            }
        }

        private void UpdateCounter()
        {
            if (saveDataObject.CrossGame.SoulsCount <= 0)
            {
                gameObject.SetActive(false);
                return;
            }


            gameObject.SetActive(true);
            text.text = saveDataObject.CrossGame.SoulsCount.ToString();
        }
    }
}
