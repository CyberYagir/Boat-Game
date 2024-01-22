using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterWindow : AnimatedWindow
    {
        
      
        
        [SerializeField] private UICharacterPreview characterPreview;
        [SerializeField] private TMP_Text charNameText;
        [SerializeField] private UIBar expBar, healthBar, thirstyBar, hungerBar;
        [SerializeField] private UISkillsSubWindow skillsSubWindow;
        [SerializeField] private UIInventorySubWindow inventorySubWindow;
        
        private PlayerCharacter selectedCharacter;
        private SelectionService selectionService;
        private TickService tickService;
        private GameDataObject gameDataObject;

        public void Init(
            SelectionService selectionService, 
            GameDataObject gameDataObject, 
            TickService tickService,
            RaftBuildService raftBuildService,
            UIMessageBoxManager uiMessageBoxManager)
        {
            this.gameDataObject = gameDataObject;
            this.tickService = tickService;
            this.selectionService = selectionService;
         
            characterPreview.Init(gameDataObject);
            
            skillsSubWindow.Init(gameDataObject, selectionService, uiMessageBoxManager);
            inventorySubWindow.Init(this.gameDataObject, selectionService, raftBuildService);
            
            
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var viewCharacter = obj.GetCharacterAction<CharActionViewCharacter>();
            viewCharacter.OnOpenWindow -= ShowWindow;
            viewCharacter.OnOpenWindow += ShowWindow;
            
            obj.NeedManager.OnDeath -= OnDeath;
            obj.NeedManager.OnDeath += OnDeath;
        }

        private void OnDeath(Character obj)
        {
            CloseWindow();
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            selectedCharacter = selectionService.SelectedCharacter;
            
            characterPreview.gameObject.SetActive(true);
            characterPreview.UpdateCharacterVisuals(selectedCharacter.Character);
            Redraw();

            tickService.OnTick += UpdateWindow;
            
        }

        private void UpdateWindow(float time)
        {
            healthBar.UpdateBar(selectedCharacter.NeedManager.Health);
            hungerBar.UpdateBar(selectedCharacter.NeedManager.Hunger);
            thirstyBar.UpdateBar(selectedCharacter.NeedManager.Thirsty);
            expBar.Init("lvl " + (selectedCharacter.Character.SkillData.Level + 1), selectedCharacter.Character.SkillData.Xp, gameDataObject.GetLevelXP(selectedCharacter.Character.SkillData.Level));
        }

        public void Redraw()
        {
            charNameText.text = selectedCharacter.Character.Name;

            healthBar.Init("Health", selectedCharacter.NeedManager.Health, 100f);
            hungerBar.Init("Hunger", selectedCharacter.NeedManager.Hunger, 100f);
            thirstyBar.Init("Thirsty", selectedCharacter.NeedManager.Thirsty, 100f);

            skillsSubWindow.Redraw(selectedCharacter.Character);
            inventorySubWindow.Redraw();
        }


        public override void CloseWindow()
        {
            base.CloseWindow();
            characterPreview.gameObject.SetActive(false);
            tickService.OnTick -= UpdateWindow;

            if (selectedCharacter != null)
            {
                selectedCharacter.NeedManager.OnDeath -= OnDeath;
            }
        }
    }
}
