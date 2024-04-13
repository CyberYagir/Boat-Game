using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIMessageBoxWindow : AnimatedWindow
    {
        [SerializeField] private TMP_Text yesText, noText, mainText;
        [SerializeField] private Button yesButton, noButton;

        
        public void Init(string text, string okText = "Yes", string noText = "No")
        {
            yesText.text = okText;
            this.noText.text = noText;
            mainText.text = text;
        }

        public void AddActions(Action yesAction)
        {
            yesButton.onClick.AddListener(delegate { yesAction?.Invoke(); });
        }

        public void SetOrder(int order)
        {
            GetComponent<Canvas>().sortingOrder = order;
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Destroy(gameObject);
        }
    }
}
