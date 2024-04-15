using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftingTableWindow : AnimatedWindow
    {
        [SerializeField] private UICraftingTableItem itemPrefab;
        [SerializeField, ReadOnly] private List<UICraftsItem> craftsItems = new List<UICraftsItem>();

        private ResourcesService resourcesService;
        private List<CraftObject> crafts;
        private UIService uiService;
        private GameStateService gameStateService;
        private RaftBuildService raftBuildService;


        public void Init(
            SelectionService selectionService,
            GameDataObject gameDataObject,
            ResourcesService resourcesService,
            UIService uiService,
            GameStateService gameStateService,
            RaftBuildService raftBuildService
        )
        {
            this.raftBuildService = raftBuildService;
            this.gameStateService = gameStateService;
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
        }

        private void UpdateItems()
        {
            foreach (var uiCraftsItem in craftsItems)
            {
                uiCraftsItem.UpdateItem();
            }
        }
    }
}
