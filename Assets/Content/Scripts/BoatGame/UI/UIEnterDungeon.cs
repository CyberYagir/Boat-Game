using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Map.UI;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIEnterDungeon : AnimatedWindow
    {
        [SerializeField] private UIMark mark;
        
        private Crypt selectedCrypt;
        private UIService uiService;

        public void Init(SelectionService selectionService, UIService uiService)
        {
            this.uiService = uiService;
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
            
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var furnaceAction = obj.GetCharacterAction<CharActionEnterDungeon>();
            furnaceAction.OnOpenWindow -= ShowWindow;
            furnaceAction.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        private void ShowWindow(Crypt obj)
        {
            selectedCrypt = obj;
            mark.Init(obj.Level, obj.Name);
            base.ShowWindow();
        }

        public void EnterDungeonButton()
        {
            uiService.EnterDungeon(selectedCrypt.Seed);
        }
    }
}
