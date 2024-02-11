using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIIslandWindow : AnimatedWindow
    {
        [SerializeField] private Animator iconAnimator;
        [SerializeField] private UIMark mark;

        public void Init(MapSelectionService selectionService)
        {
            selectionService.OnSelectIsland += SelectionServiceOnOnSelectIsland;
        }

        private void SelectionServiceOnOnSelectIsland(MapIsland obj)
        {
            mark.Init(obj.GeneratedData);
            ShowWindow();
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
            
            TimeService.SetTimeRate(1f);
        }
    }
}
