using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UITabsButtons : MonoBehaviour
    {
        [SerializeField] private List<DOTweenAnimation> buttons;

        private void Awake()
        {
            GetComponent<ITabManager>().OnTabChanged += Select;
        }

        public void Select(int target)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (target != i)
                {
                    buttons[i].DORewind();
                }
            }

            buttons[target].DOPlay();
        }
    }
}
