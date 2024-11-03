using System;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollUp : MonoBehaviour
    {
        private ScrollRect rect;

        private void Awake()
        {
            rect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            ToTop();
        }

        public void ToTop()
        {
            rect.content.anchoredPosition = Vector2.zero;
        }
    }
}
