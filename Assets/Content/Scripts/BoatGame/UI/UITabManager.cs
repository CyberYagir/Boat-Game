using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Content.Scripts.BoatGame.UI
{
    public class UITabManager : MonoBehaviour
    {
        [System.Serializable]
        public class Tab
        {
            [SerializeField] private GameObject page;
            [SerializeField] private UnityEvent OnTabSelected;
            [SerializeField] private UnityEvent OnTabClosed;

            public GameObject Page => page;

            public void Close()
            {
                OnTabClosed.Invoke();
            }

            public void Open()
            {
                OnTabSelected.Invoke();
            }

            public void SetState(bool b)
            {
                page.gameObject.SetActive(b);
            }
        }
        [SerializeField, ReadOnly] private int selectedTab = -1;
        [SerializeField] private List<Tab> tabs;


        private void OnEnable()
        {
            DOVirtual.DelayedCall(0.1f, delegate
            {
                SelectTab(0);
            });
        }


        public void SelectTab(int indx)
        {
            if (indx != selectedTab)
            {
                if (selectedTab != -1)
                {
                    tabs[selectedTab].Close();
                }

                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].SetState(i == indx);
                }

                selectedTab = indx;
                tabs[selectedTab].Open();
            }
        }
    }
}