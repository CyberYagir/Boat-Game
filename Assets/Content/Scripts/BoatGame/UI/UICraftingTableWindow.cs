using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftingTableWindow : AnimatedWindow, ITabManager
    {
        [SerializeField] private UICraftingTableItem itemPrefab;
        [SerializeField, ReadOnly] private List<UICraftsItem> craftsItems = new List<UICraftsItem>();

        private ResourcesService resourcesService;
        private List<CraftObject> crafts;
        private UIService uiService;
        private RaftBuildService raftBuildService;

        private List<CraftObject.ECraftSubList> actualToDraw = new List<CraftObject.ECraftSubList>();
        
        public event Action<int> OnTabChanged;



        public void Init(
            SelectionService selectionService,
            GameDataObject gameDataObject,
            ResourcesService resourcesService,
            UIService uiService,
            RaftBuildService raftBuildService
        )
        {
            this.raftBuildService = raftBuildService;
            this.uiService = uiService;
            this.resourcesService = resourcesService;

            crafts = gameDataObject.Crafts.FindAll(x => x.CraftType == CraftObject.ECraftType.Item);


            resourcesService.OnChangeResources += OnChangeResources;
            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            OnChangeSelectCharacter(selectionService.SelectedCharacter);
            RedrawItems();
        }
        
        private void OnChangeResources()
        {
            if (IsOpen)
            {
                resourcesService.PlayerItemsList();
                UpdateItems();
            }
        }
        

        private void RedrawItems()
        {
            itemPrefab.gameObject.SetActive(true);
            foreach (var craftObject in crafts)
            {
                Instantiate(itemPrefab, itemPrefab.transform.parent)
                    .With(x => x.Init(craftObject, resourcesService, uiService, raftBuildService))
                    .With(x => craftsItems.Add(x));
            }

            itemPrefab.gameObject.SetActive(false);
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            if (obj == null) return;
            var viewCharacter = obj.GetCharacterAction<CharActionCraft>();
            viewCharacter.OnShowBuildWindow -= ShowWindow;
            viewCharacter.OnShowBuildWindow += ShowWindow;
            
            viewCharacter.OnCloseBuildWindow -= CloseWindow;
            viewCharacter.OnCloseBuildWindow += CloseWindow;
            
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
            resourcesService.PlayerItemsList();

            UpdateItems();
            ChangeTab(0);
        }

        private void UpdateItems()
        {
            foreach (var uiCraftsItem in craftsItems)
            {
                uiCraftsItem.UpdateItem();
                uiCraftsItem.gameObject.SetActive(actualToDraw.Contains(uiCraftsItem.Item.SubType));
            }
        }


        public void ChangeTab(int value)
        {
            actualToDraw.Clear();
            switch (value)
            {
                case 0:
                    actualToDraw.Add(CraftObject.ECraftSubList.Armor, CraftObject.ECraftSubList.Materials, CraftObject.ECraftSubList.Money);
                    break;
                case 1:
                    actualToDraw.Add(CraftObject.ECraftSubList.Armor);
                    break;
                case 2:
                    actualToDraw.Add(CraftObject.ECraftSubList.Materials);
                    break;
                case 3:
                    actualToDraw.Add(CraftObject.ECraftSubList.Money);
                    break;
            }
            
            UpdateItems();
            
            OnTabChanged?.Invoke(value);
        }
    }
}
