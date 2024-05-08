using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIFurnaceWindow : AnimatedWindow
    {
        [SerializeField] private UIFurnaceInventorySubWindow inventorySubWindow;
        [SerializeField] private List<UIFurnaceDestination> slots;
        public void Init(
            SelectionService selectionService,
            RaftBuildService raftBuildService
        )
        {
            inventorySubWindow.Init(raftBuildService);

            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }
        
        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionFurnace>();
            furnaceAction.OnOpenWindow -= ShowWindow;
            furnaceAction.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            Redraw();
        }

        private void Redraw()
        {
            inventorySubWindow.Redraw();
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Init(inventorySubWindow);
            }
        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }
    }
}
