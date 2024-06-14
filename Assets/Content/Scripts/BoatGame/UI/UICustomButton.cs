using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UICustomButton : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Button button;
        [SerializeField] private Color activeColor = new Color(0.3380358f, 1f, 0, 1f);
        [SerializeField] private Color disabledColor = new Color(0.4f, 0.4f, 0.4f, 1f);

        public Button Button => button;

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }

        public void SetInteractable(bool state)
        {
            button.interactable = state;
            button.image.DOColor(button.interactable ? activeColor : disabledColor, 0.2f).SetLink(button.gameObject);
        }

        public void SetTransparent()
        {
            button.interactable = false;
            button.image.DOColor(activeColor, 0.2f).SetLink(button.gameObject);
        }

        public void Click()
        {
            button.onClick.Invoke();
        }
    }
}
