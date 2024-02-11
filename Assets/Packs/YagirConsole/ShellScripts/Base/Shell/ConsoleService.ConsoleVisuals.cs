using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Packs.YagirConsole.ShellScripts.Base.Shell
{
    public partial class ConsoleService
    {
        [System.Serializable]
        public class ConsoleVisuals
        {
            [SerializeField] private Canvas canvas;
            
            [SerializeField] private TMP_InputField input;
            [SerializeField] private TMP_Text outputText;
            
            [Space]
            [SerializeField] private Color consoleColor;
            [SerializeField] private Color selectedColor;
            [SerializeField] private Color selectedColorText;
            [SerializeField] private List<Image> backgrounds;
            
            
            private RectTransform rectTransform;

            public TMP_InputField Input => input;

            public TMP_Text OutputText => outputText;

            public Color SelectedColor => selectedColor;

            public Color SelectedColorText => selectedColorText;

            public void Init()
            {
                rectTransform = canvas.GetComponent<RectTransform>();
                canvas.enabled = false;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                OutputText.gameObject.SetActive(false);
                
                                
                for (int i = 0; i < backgrounds.Count; i++)
                {
                    backgrounds[i].color = consoleColor;
                }
                
                AnimateHide();
            }


            public void AnimateHide()
            {
                rectTransform
                    .DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 0), 0.2f)
                    .SetLink(rectTransform.gameObject).onComplete += () =>
                {
                    canvas.enabled = false;
                    outputText.gameObject.SetActive(false);
                };
            }

            public void AnimateShow()
            {
                rectTransform.DOKill();
                canvas.enabled = true;
                outputText.gameObject.SetActive(true);
                rectTransform
                    .DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, 500), 0.2f) 
                    .SetLink(rectTransform.gameObject);
                input.Select();
            }
        }
    }
}