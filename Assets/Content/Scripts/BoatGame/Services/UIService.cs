using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Boot;
using Content.Scripts.CraftsSystem;
using Content.Scripts.DungeonGame.UI;
using Content.Scripts.Global;
using Content.Scripts.ManCreator;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class UIService : MonoBehaviour
    {
        [System.Serializable]
        public class WindowsManager
        {
            [SerializeField, ReadOnly] private List<AnimatedWindow> openedWindows = new List<AnimatedWindow>(2);
            public bool isAnyWindowOpened => openedWindows.Count != 0;

            public void Init(UIService uiService, params AnimatedWindow[] windows)
            {
                for (int i = 0; i < windows.Length; i++)
                {
                    if (windows[i] != null)
                    {
                        windows[i].OnOpen += delegate(AnimatedWindow window) { openedWindows.Add(window); };
                        windows[i].OnClose += delegate(AnimatedWindow window) { openedWindows.Remove(window); };

                        windows[i].InitWindow(uiService);
                    }
                }
            }
        }

        [SerializeField] private UIActionManager actionManager;
        [SerializeField] private UIRewindButton rewindButton;
        [SerializeField] private UIExitIslandButton exitIslandButton;
        [SerializeField] private UIMapButton mapButton;
        [SerializeField] private UIStopBuildButton stopBuildButton;
        [SerializeField] private UIOptionsHolder optionsHolder;
        [SerializeField] private UIChestShow chestShow;
        [SerializeField] private UIPotionsList potionsList;
        [SerializeField] private UICraftsWindow craftsWindow;
        [SerializeField] private UICraftingTableWindow craftingTableWindow;
        [SerializeField] private UICharacterWindow characterWindow;
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private UIDeathWindow deathWindow;
        [SerializeField] private UICharactersList charactersList;
        [SerializeField] private UIResourcesCounter resourcesList;
        [SerializeField] private UIStoragesCounter storagesCounter;
        [SerializeField] private UIFurnaceWindow furnaceWindow;
        [SerializeField] private UIVillageOptionsWindow villageWindow;
        [SerializeField] private UILoreScrollWindow loreScrollWindow;
        [SerializeField] private RenameIslandWindow renameIslandWindow;
        [SerializeField] private UIGetScrollWindow getScrollWindow;
        
        [Space, SerializeField] private WindowsManager windowsManager = new WindowsManager();
        
        private PlayerCharacter targetCharacter;
        private ResourcesService resourcesService;
        private GameStateService gameState;

        public WindowsManager WindowManager => windowsManager;

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
            SaveService saveService,
            PrefabSpawnerFabric spawnerFabric, 
            PlayerAuthService authService,
            CloudService cloudService
        )
        {
            this.gameState = gameState;
            this.resourcesService = resourcesService;


            deathWindow.Init(characterService, saveDataObject, scenesService);
            
            actionManager.Init(selectionService);
            rewindButton.Init(tickService, gameStateService, this);
            mapButton.Init(raftBuildService, scenesService, saveService);
            stopBuildButton.Init(tickService, gameStateService, this);
            exitIslandButton.Init(messageBoxManager, saveService, scenesService);
            optionsHolder.Init(storagesCounter, authService, saveService, cloudService, scenesService, messageBoxManager, saveDataObject);
            chestShow.Init(gameDataObject, selectionService);
            craftsWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService);
            craftingTableWindow.Init(selectionService, gameDataObject, this.resourcesService, this, raftBuildService);
            characterWindow.Init(selectionService, gameDataObject, tickService, raftBuildService, messageBoxManager, spawnerFabric, resourcesService);
            furnaceWindow.Init(selectionService, raftBuildService, tickService, resourcesService);
            renameIslandWindow.Init(saveDataObject);
            getScrollWindow.Init(gameDataObject, resourcesService);
            charactersList?.Init(characterService, tickService, selectionService);
            
            
            if (saveDataObject.Global.isOnIsland)
            {
                if (saveDataObject.GetTargetIsland().HasVillage())
                {
                    villageWindow?.Init(
                        selectionService,
                        raftBuildService,
                        saveDataObject,
                        gameDataObject,
                        resourcesService,
                        tickService,
                        this,
                        messageBoxManager,
                        scenesService,
                        saveService);
                }

                if (saveDataObject.Map.IsHavePlotOnIsland(saveDataObject.GetTargetIsland().IslandSeed))
                {
                    loreScrollWindow.Init(selectionService, saveDataObject, gameDataObject, this.resourcesService);
                }
            }

            resourcesList.Init(raftBuildService, gameDataObject, resourcesService, tickService);
            storagesCounter.Init(raftBuildService);

            potionsList.Init(resourcesService, selectionService, characterService, charactersList, scenesService);

            windowsManager.Init(this, craftsWindow, characterWindow, craftingTableWindow, furnaceWindow, villageWindow, loreScrollWindow);
            
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
        public void ChangeGameStateToRemove()
        {
            gameState.ChangeGameState(GameStateService.EGameState.Removing);
        }

        public void CharacterCraftItem(CraftObject item)
        {
            targetCharacter.GetCharacterAction<CharActionCraft>().SetCraft(item);
            targetCharacter.ActiveAction(EStateType.Crafting);
        }


        public void SetResourcesCounterSorting(int i)
        {
            resourcesList.SetOverrideSorting(i);
        }
    }
}
