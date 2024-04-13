using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIMessageBoxManager : MonoBehaviour
    {
        [SerializeField] private UIMessageBoxWindow windowPrefab;
        [SerializeField] private Image blocker;
        private List<UIMessageBoxWindow> openedPopups = new List<UIMessageBoxWindow>();

        private int order = 5;
        
        


        public void ShowMessageBox(string text, Action okAction, string okText = "Yes", string noText = "No")
        {
            blocker.enabled = true;
            Instantiate(windowPrefab, transform)
                .With(x => x.Init(text, okText, noText))
                .With(x => x.AddActions(okAction))
                .With(x=>x.SetOrder(order))
                .With(x=>x.ShowWindow())
                .With(x=>openedPopups.Add(x))
                .With(x=>x.OnClose += OnMessageBoxClosed);
            order++;
        }

        private void OnMessageBoxClosed(AnimatedWindow obj)
        {
            openedPopups.Remove(obj as UIMessageBoxWindow);
            
            if (openedPopups.Count == 0)
            {
                blocker.enabled = false;
            }
        }
    }
}
