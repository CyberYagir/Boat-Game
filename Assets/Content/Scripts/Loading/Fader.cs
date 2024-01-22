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
            transform.parent = null;
            image.enabled = true;
            image.raycastTarget = true;
            DontDestroyOnLoad(gameObject);
            image.DOFade(1, 0.5f).onComplete += delegate
            {
                action?.Invoke();
                UnFade();
            };
        }

        public void UnFade()
        {
            image.DOFade(0, 0.5f).onComplete += delegate
            {
                Destroy(gameObject);
            };
        }
    }
}
