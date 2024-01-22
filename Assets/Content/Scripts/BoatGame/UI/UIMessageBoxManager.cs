using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIMessageBoxManager : MonoBehaviour
    {
        [SerializeField] private UIMessageBoxWindow windowPrefab;
        private int order = 5;



        public void ShowMessageBox(string text, Action okAction, string okText = "Yes", string noText = "No")
        {
            Instantiate(windowPrefab, transform)
                .With(x => x.Init(text, okText, noText))
                .With(x => x.AddActions(okAction))
                .With(x=>x.SetOrder(order))
                .With(x=>x.ShowWindow());
            order++;
        }
    }
}
