using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class UIService : MonoBehaviour
    {
        [SerializeField] private UIActionManager actionManager;
        [SerializeField] private UIRewindButton rewindButton;
        [SerializeField] private UIStopBuildButton stopBuildButton;
        [SerializeField] private UIChestShow chestShow;
        [SerializeField] private UICraftsWindow craftsWindow;
        [SerializeField] private UICraftingTableWindow craftingTableWindow;
        [SerializeField] private UICharacterWindow characterWindow;
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private List<ResourcesCounter> counter;
        
        
        private PlayerCharacter targetCharacter;
        private TickService tickService;
        private ResourcesService resourcesService;
        private GameStateService _gameState;

        [Inject]
        private void Construct(
            SelectionService selectionService,
            TickService tickService,
            ResourcesService resourcesService,
            GameDataObject gameDataObject,
            GameStateService gameState,
            GameStateService gameStateService,
            CharacterService characterService,
            RaftBuildService raftBuildService
        )
        {
            this._gameState = gameState;
            this.resourcesService = resourcesService;
            this.tickService = tickService;

            actionManager.Init(selectionService);
            rewindButton.Init(tickService, gameStateService);
            stopBuildButton.Init(tickService, gameStateService);
            
            chestShow.Init(gameDataObject, selectionService);
            craftsWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService);
            craftingTableWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService);
            characterWindow.Init(selectionService, gameDataObject, tickService, raftBuildService, messageBoxManager);
                
                
            selectionService.OnChangeSelectCharacter += ChangeCharacter;

            resourcesService.OnChangeResources += OnChangeResources;


            ChangeCharacter(selectionService.SelectedCharacter);

            for (int i = 0; i < counter.Count; i++)
            {
                var data = resourcesService.GetResourceData(counter[i].ResourceTypes);
                counter[i].UpdateCounter(data.Count, data.MaxCount);
            }
        }

        private void ChangeCharacter(PlayerCharacter newPlayer)
        {
            actionManager.UpdateButtons(false);
            if (newPlayer != targetCharacter)
            {
                if (targetCharacter != null)
                {
                    targetCharacter.OnChangeState -= OnChangeState;
                }

                if (newPlayer != null)
                {
                    newPlayer.OnChangeState += OnChangeState;
                    targetCharacter = newPlayer;
                }
            }
        }

        private void OnChangeState()
        {
            actionManager.UpdateButtons(false);
        }

        private void OnChangeResources(EResourceTypes name, RaftStorage.ResourceTypeHolder data)
        {
            var count = counter.Find(x => x.ResourceTypes == name);
            if (count != null)
            {
                count.UpdateCounter(data.Count, data.MaxCount);
            }
        }

        private void LateUpdate()
        {
            actionManager.Update();
            chestShow.Update();
        }

        public void ChangeGameStateToBuild()
        {
            _gameState.ChangeGameState(GameStateService.EGameState.Building);
        }

        public void CharacterCraftItem(CraftObject item)
        {
            targetCharacter.GetCharacterAction<CharActionCraft>().SetCraft(item);
            targetCharacter.ActiveAction(EStateType.Crafting);
        }
    }
}
