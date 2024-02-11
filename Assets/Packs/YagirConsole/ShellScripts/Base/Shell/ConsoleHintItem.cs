using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    public class ConsoleHintItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image background;


        private Color defaultTextColor, defaultBackgroundColor;
        private Color activeTextColor, activeBackgroundColor;
        
        public void Init(Color consoleVisualsSelectedColor, Color consoleVisualsSelectedColorText)
        {
            defaultTextColor = text.color;
            defaultBackgroundColor = background.color;

            activeTextColor = consoleVisualsSelectedColorText;
            activeBackgroundColor = consoleVisualsSelectedColor;
        }
        
        [SerializeField] private bool selected;

        public void Select()
        {
            if (!selected)
            {
                text.DOColor(activeTextColor, 0.2f).SetLink(gameObject);
                background.DOColor(activeBackgroundColor, 0.2f).SetLink(gameObject);
                selected = true;
            }
        }

        public void Deselect()
        {
            if (selected)
            {
                text.DOColor(defaultTextColor, 0.2f).SetLink(gameObject);
                background.DOColor(defaultBackgroundColor, 0.2f).SetLink(gameObject);
                selected = false;
            }
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public string GetText()
        {
            return text.text;
        }

   
    }
}
