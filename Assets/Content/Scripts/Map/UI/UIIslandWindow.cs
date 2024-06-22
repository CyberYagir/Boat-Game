using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIIslandWindow : AnimatedWindow
    {
        [SerializeField] private Animator iconAnimator;
        [SerializeField] private UIMark mark;
        private MapSelectionService mapSelectionService;

        public void Init(MapSelectionService selectionService)
        {
            mapSelectionService = selectionService;
            selectionService.OnSelectIsland += SelectionServiceOnOnSelectIsland;
        }

        private void SelectionServiceOnOnSelectIsland(MapIsland obj)
        {
            if (!IsOpen)
            {
                mapSelectionService.enabled = false;
                mark.Init(obj.GeneratedData, String.Empty);
                ShowWindow();
            }
        }

        public override void ShowWindow()
        {
            gameObject.SetActive(true);
            base.ShowWindow();
            
            TimeService.SetTimeRate(0);
            
            iconAnimator.Play("Start");
        }

        public override void OnClosed()
        {
            base.OnClosed();
            mapSelectionService.enabled = true;
            TimeService.SetTimeRate(1f);
        }

        public void GoToIsland()
        {
            mapSelectionService.LoadIsland();
        }
    }
}
