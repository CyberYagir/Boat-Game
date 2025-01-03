using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using TMPro;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharacterWindow : AnimatedWindow
    {
        [SerializeField] private UICharacterPreview characterPreview;
        [SerializeField] private TMP_Text charNameText;
        [SerializeField] private UIBar expBar, healthBar, thirstyBar, hungerBar;
        [SerializeField] private UISkillsSubWindow skillsSubWindow;
        [SerializeField] private UICharacterInventorySubWindow inventorySubWindow;
        [SerializeField] private UITabManager uiTabManager;
        [SerializeField] private UICharacterWindowStats statsTray;
        [SerializeField] private UICharacterWindowStrength strengthDisplay;
        [SerializeField] private List<UIEquipmentDestination> equipmentBases;

        private PlayerCharacter selectedCharacter;
        private ISelectionService selectionService;
        private TickService tickService;
        private GameDataObject gameDataObject;
        private IRaftBuildService raftBuildService;
        private IResourcesService resourcesService;

        public void Init(
            ISelectionService selectionService, 
            GameDataObject gameDataObject, 
            TickService tickService,
            IRaftBuildService raftBuildService,
            UIMessageBoxManager uiMessageBoxManager,
            PrefabSpawnerFabric spawnerFabric,
            IResourcesService resourcesService,
            IUIService uiService)
        {
            this.resourcesService = resourcesService;
            this.raftBuildService = raftBuildService;
            this.gameDataObject = gameDataObject;
            this.tickService = tickService;
            this.selectionService = selectionService;
         
            spawnerFabric.InjectComponent(characterPreview);
            
            skillsSubWindow.Init(gameDataObject, selectionService, uiMessageBoxManager);
            inventorySubWindow.Init(raftBuildService, uiService);
            
            
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var viewCharacter = obj.GetCharacterAction<CharActionViewCharacter>();
            if (viewCharacter)
            {
                viewCharacter.OnOpenWindow -= ShowWindow;
                viewCharacter.OnOpenWindow += ShowWindow;
            }

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
            selectedCharacter.Character.OnSkillUpgraded += RedrawStats;
        }

        private void RedrawStats()
        {
            statsTray.Redraw(selectedCharacter);
            strengthDisplay.Redraw(selectedCharacter);
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

            RedrawStats();
        }


        public override void CloseWindow()
        {
            base.CloseWindow();
            characterPreview.gameObject.SetActive(false);
            tickService.OnTick -= UpdateWindow;

            if (selectedCharacter != null)
            {
                selectedCharacter.Character.OnSkillUpgraded -= RedrawStats;
                selectedCharacter.NeedManager.OnDeath -= OnDeath;
            }
        }

        public bool ChangeEquipment(ItemObject item, EEquipmentType type)
        {
            if (item != null)
            {
                if (item.Type != EResourceTypes.Other || item.Equipment == EEquipmentType.None) return false;
                if (type != item.Equipment) return false;
            }

            var equipment = gameDataObject.GetItem(selectedCharacter.Character.Equipment.GetEquipment(type));

            if (equipment != null)
            {
                if (resourcesService.AddToAnyStorage(equipment))
                {
                    resourcesService.RemoveItemFromAnyRaft(item);
                    selectedCharacter.Character.Equipment.SetEquipment(item, type);
                    Redraw();
                    return true;
                }
                else if (item != null) //for creative items overflow
                {
                    SwapToStorage(item, equipment);
                    selectedCharacter.Character.Equipment.SetEquipment(item, type);
                    Redraw();
                    return true;
                }
            }
            else if (item != null)
            {
                selectedCharacter.Character.Equipment.SetEquipment(item, type);
                resourcesService.RemoveItemFromAnyRaft(item);
                Redraw();
                return true;
            }

            Redraw();
            return false;
        }

        private void SwapToStorage(ItemObject item, ItemObject equipment)
        {
            resourcesService.TrySwapItems(new RaftStorage.StorageItem(item, 1), new RaftStorage.StorageItem(equipment, 1));
        }


        public void ChangeTabToInventory()
        {
            uiTabManager.SelectTab(1);
        }
    }
}
