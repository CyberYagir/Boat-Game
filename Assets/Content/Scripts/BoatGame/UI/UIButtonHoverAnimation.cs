using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Content.Scripts.BoatGame.UI
{
    public class UIButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private DOTweenAnimation animation;
        private void Awake()
        {
            animation = GetComponent<DOTweenAnimation>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter();
        }

        private void OnEnter()
        {
            animation.DOPlayForward();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit();
        }

        private void OnExit()
        {
            animation.DOPlayBackwards();
        }
    }
}
