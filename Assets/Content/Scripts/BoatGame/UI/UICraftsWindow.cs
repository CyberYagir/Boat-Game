using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICraftsWindow : AnimatedWindow
    {
        [SerializeField] private UICraftsItem itemPrefab;

        [SerializeField, ReadOnly] private List<UICraftsItem> craftsItems = new List<UICraftsItem>();


        private GameDataObject gameDataObject;
        private IResourcesService resourcesService;
        private UIService uiService;
        private IRaftBuildService raftBuildService;
        private List<CraftObject> crafts = new List<CraftObject>();
        private SaveDataObject saveDataObject;


        public void Init(
            SelectionService selectionService,
            GameDataObject gameDataObject,
            IResourcesService resourcesService,
            UIService uiService,
            GameStateService gameStateService,
            IRaftBuildService raftBuildService,
            SaveDataObject saveDataObject
        )
        {
            this.saveDataObject = saveDataObject;
            this.raftBuildService = raftBuildService;
            this.uiService = uiService;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;

            selectionService.OnChangeSelectCharacter += OnChangeSelectCharacter;
            gameStateService.OnChangeEState += GameStateServiceOnOnChangeState;
            resourcesService.OnChangeResources += OnChangeResources;

            crafts = gameDataObject.Crafts.FindAll(x => x.CraftType == CraftObject.ECraftType.Raft);
            
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

        private void GameStateServiceOnOnChangeState(GameStateService.EGameState eGameState)
        {
            if (eGameState != GameStateService.EGameState.Normal)
            {
                CloseWindow();
            }
        }

        private void OnChangeSelectCharacter(PlayerCharacter obj)
        {
            var buildAction = obj.GetCharacterAction<CharActionBuilding>();
            buildAction.OnShowBuildWindow -= OnShowBuildWindow;
            buildAction.OnShowBuildWindow += OnShowBuildWindow;
        }

        private void OnShowBuildWindow()
        {
            ShowWindow();
        }

        public override void ShowWindow()
        {
            base.ShowWindow();
            resourcesService.PlayerItemsList();

            UpdateItems();
        }

        public void RedrawItems()
        {
            itemPrefab.gameObject.SetActive(true);
            foreach (var craftObject in crafts)
            {
                Instantiate(itemPrefab, itemPrefab.transform.parent)
                    .With(x => x.Init(craftObject, resourcesService, uiService, raftBuildService, saveDataObject))
                    .With(x => craftsItems.Add(x));
            }

            itemPrefab.gameObject.SetActive(false);
        }

        public void UpdateItems()
        {
            foreach (var uiCraftsItem in craftsItems)
            {
                if (uiCraftsItem.UpdateItem())
                {
                    uiCraftsItem.transform.SetSiblingIndex(0);
                }
            }
        }

        public void RemoveRafts()
        {
            CloseWindow();
            uiService.ChangeGameStateToRemove();
        }
    }
}