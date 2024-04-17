using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
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
        [SerializeField] private UITabManager uiTabManager;
        [SerializeField] private List<UIEquipmentBase> equipmentBases;

        private PlayerCharacter selectedCharacter;
        private SelectionService selectionService;
        private TickService tickService;
        private GameDataObject gameDataObject;
        private RaftBuildService raftBuildService;

        public void Init(
            SelectionService selectionService, 
            GameDataObject gameDataObject, 
            TickService tickService,
            RaftBuildService raftBuildService,
            UIMessageBoxManager uiMessageBoxManager)
        {
            this.raftBuildService = raftBuildService;
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
            
            for (int i = 0; i < equipmentBases.Count; i++)
            {
                equipmentBases[i].Init(selectedCharacter.Character, gameDataObject, this, inventorySubWindow);
            }
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

        public void ChangeEquipment(ItemObject item, UIEquipmentBase.EEquipmentType type)
        {
            if (item != null)
            {
                if (item.Type != EResourceTypes.Other) return;
                if (type != item.Equipment) return;
            }

            var equipment = gameDataObject.GetItem(selectedCharacter.Character.Equipment.GetEquipment(type));

            selectedCharacter.Character.Equipment.SetEquipment(item, type);
            RemoveFromStorage(item);

            if (equipment != null)
            {
                AddToStorage(equipment);
            }
            
            Redraw();
        }

        public void RemoveFromStorage(ItemObject item)
        {
            if (item == null) return;
            foreach (var raftStorage in raftBuildService.Storages)
            {
                if (raftStorage.RemoveFromStorage(item))
                {
                    break;
                }
            }
        }
        public void AddToStorage(ItemObject item)
        {
            foreach (var raftStorage in raftBuildService.Storages)
            {
                if (raftStorage.IsEmptyStorage(1))
                {
                    raftStorage.AddToStorage(item, 1);
                    break;
                }
            }
        }

        public void ChangeTabToInventory()
        {
            uiTabManager.SelectTab(1);
        }
    }
}
