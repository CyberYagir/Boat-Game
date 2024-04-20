using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private GameObject tooltip;
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private List<ContentSizeFitter> contentSizeFitters;
        
        private float timer;
        private IEnumerator loop;
        private float tooltipShowTime = 0.5f;
        private TooltipDataObject tooltipData;
        private bool isStartAnimation;

        public void Init(TooltipDataObject tooltipData)
        {
            this.tooltipData = tooltipData;
            tooltip.gameObject.SetActive(false);
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
                SetPosition();
                yield return null;

            }

            StartCoroutine(StartTooltip());
        }

        IEnumerator StartTooltip()
        {
            if (isStartAnimation) yield break;

            isStartAnimation = true;
            group.alpha = 0;
            tooltip.gameObject.SetActive(true);
            text.text = tooltipData.Text;

            for (int i = 0; i < contentSizeFitters.Count; i++)
            {
                contentSizeFitters[i].SetLayoutHorizontal();
                contentSizeFitters[i].SetLayoutVertical();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            
            SetPosition();
            
            yield return null;
            yield return null;
            
            group.DOFade(1f, 0.25f);
            
            
            while (true)
            {
                yield return null;

                SetPosition();
            }
        }

        private void SetPosition()
        {
            tooltip.transform.position = InputService.MousePosition;

            var pos = tooltip.transform.position.x + tooltip.GetComponent<RectTransform>().sizeDelta.x;
            if (pos > Screen.width)
            {
                tooltip.transform.position += Vector3.right * (Screen.width - pos);
            }
        }

        public void OnExit()
        {
            if (loop != null)
            {
                StopCoroutine(loop);
            }
            
            isStartAnimation = false;
            group.DOFade(0f, 0.25f).onComplete += delegate
            {
                tooltip.gameObject.SetActive(false);
            };
            timer = 0;
        }

        public void OnClick()
        {
            StartCoroutine(StartTooltip());
        }
        
    }
}
