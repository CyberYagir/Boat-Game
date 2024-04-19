using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private GameObject tooltip;
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup group;
        private float timer;
        private IEnumerator loop;
        private float tooltipShowTime = 0.5f;

        public void Init(TooltipDataObject tooltipData)
        {
            tooltip.gameObject.SetActive(false);
            text.text = tooltipData.Text;
        }
        

        public void OnHover()
        {
            loop = WaitForTooltip();
            StartCoroutine(loop);
        }

        IEnumerator WaitForTooltip()
        {
            while (timer <= tooltipShowTime)
            {
                timer += Time.unscaledDeltaTime;
                tooltip.transform.position = InputService.MousePosition;
                yield return null;
                
            }

            group.alpha = 0;
            tooltip.gameObject.SetActive(true);
            
            yield return null;
            yield return null;
            
            group.DOFade(1f, 0.25f);
            
            
            while (true)
            {
                yield return null;

                tooltip.transform.position = InputService.MousePosition;
            }
        }

        public void OnExit()
        {
            if (loop != null)
            {
                StopCoroutine(loop);
            }
            group.DOFade(0f, 0.25f).onComplete += delegate
            {
                tooltip.gameObject.SetActive(false);
            };
            timer = 0;
        }

        public void OnClick()
        {
            tooltip.gameObject.SetActive(true);
        }
        
    }
}
