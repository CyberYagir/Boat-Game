using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Boot;
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
        [SerializeField] private UIExitIslandButton exitIslandButton;
        [SerializeField] private UIMapButton mapButton;
        [SerializeField] private UIStopBuildButton stopBuildButton;
        [SerializeField] private UIChestShow chestShow;
        [SerializeField] private UICraftsWindow craftsWindow;
        [SerializeField] private UICraftingTableWindow craftingTableWindow;
        [SerializeField] private UICharacterWindow characterWindow;
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private UIDeathWindow deathWindow;
        [SerializeField] private UICharactersList charactersList;
        [SerializeField] private UIResourcesCounter resourcesList;
        
        private PlayerCharacter targetCharacter;
        private TickService tickService;
        private ResourcesService resourcesService;
        private GameStateService gameState;

        [Inject]
        private void Construct(
            SelectionService selectionService,
            TickService tickService,
            ResourcesService resourcesService,
            GameDataObject gameDataObject,
            GameStateService gameState,
            GameStateService gameStateService,
            CharacterService characterService,
            RaftBuildService raftBuildService, 
            SaveDataObject saveDataObject,
            ScenesService scenesService,
            SaveService saveService
        )
        {
            this.gameState = gameState;
            this.resourcesService = resourcesService;
            this.tickService = tickService;


            deathWindow.Init(characterService, saveDataObject, scenesService);
            
            actionManager.Init(selectionService);
            rewindButton.Init(tickService, gameStateService);
            mapButton.Init(raftBuildService, scenesService, saveService);
            stopBuildButton.Init(tickService, gameStateService);
            exitIslandButton.Init(messageBoxManager, saveService, scenesService);
            
            chestShow.Init(gameDataObject, selectionService);
            craftsWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService);
            craftingTableWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService);
            characterWindow.Init(selectionService, gameDataObject, tickService, raftBuildService, messageBoxManager);
            charactersList?.Init(characterService, tickService, selectionService);
            
            
            resourcesList.Init(raftBuildService, gameDataObject, resourcesService, tickService);
            
            selectionService.OnChangeSelectCharacter += ChangeCharacter;
            resourcesService.OnChangeResources += OnChangeResources;


            ChangeCharacter(selectionService.SelectedCharacter);
            
        }

        private void OnChangeResources()
        {
            resourcesList.UpdateCounter();
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

        // private void OnChangeResources(EResourceTypes name, RaftStorage.StorageItem data)
        // {
        //     var count = counter.Find(x => x.ResourceTypes == name);
        //     if (count != null)
        //     {
        //         count.UpdateCounter(data.Count, data.MaxCount);
        //     }
        // }

        private void LateUpdate()
        {
            actionManager.Update();
            chestShow.Update();
        }

        public void ChangeGameStateToBuild()
        {
            gameState.ChangeGameState(GameStateService.EGameState.Building);
        }

        public void CharacterCraftItem(CraftObject item)
        {
            targetCharacter.GetCharacterAction<CharActionCraft>().SetCraft(item);
            targetCharacter.ActiveAction(EStateType.Crafting);
        }
    }
}
