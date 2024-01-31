using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Loading
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private Image image;



        public void Fade(Action action)
        {
            if (!gameObject.active)
            {
                gameObject.SetActive(true);
                
                image.SetAlpha(0);
            }

            image.DOKill();
            image.enabled = true;
            image.raycastTarget = true;
            image.DOFade(1, 0.5f).onComplete += delegate
            {
                action?.Invoke();
                UnFade();
            };
        }

        public void UnFade()
        {
            image.DOFade(0, 0.5f).SetDelay(0.25f).onComplete += delegate
            {
                gameObject.SetActive(false);
            };
        }
    }
}
