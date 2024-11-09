using System.Collections.Generic;
using System.Linq;
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
    public interface IUIService
    {
        public UIService.WindowsManager WindowManager { get; }
    }
    public class UIService : MonoBehaviour, IUIService
    {
        [System.Serializable]
        public class WindowsManager
        {
            [SerializeField, ReadOnly] private List<AnimatedWindow> openedWindows = new List<AnimatedWindow>(2);

            public bool isAnyWindowOpened => openedWindows.Count != 0;

            public void Init(IUIService uiService, TickService tickService , params AnimatedWindow[] windows)
            {
                for (int i = 0; i < windows.Length; i++)
                {
                    if (windows[i] != null)
                    {
                        windows[i].OnOpen += delegate(AnimatedWindow window)
                        {
                            openedWindows.Add(window);
                            tickService.NormalTime();
                            print("Show");
                        };
                        windows[i].OnClose += delegate(AnimatedWindow window)
                        {
                            openedWindows.Remove(window); 
                            
                            print("Close");
                        };

                        windows[i].InitWindow(uiService);
                    }
                }
            }

            public bool CloseLast()
            {
                if (openedWindows.Count != 0)
                {
                    var last = openedWindows.Last();
                    if (last != null)
                    {
                        last.CloseWindow();
                        return true;
                    }
                }

                return false;
            }
        }

        [SerializeField] private UIActionManager actionManager;
        [SerializeField] private UIRewindButton rewindButton;
        [SerializeField] private UIExitIslandButton exitIslandButton;
        [SerializeField] private UIMapButton mapButton;
        [SerializeField] private UIPlayerStorageButton playerStorageButton;
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
        [SerializeField] private UISoulsCounter soulsCounter;
        [SerializeField] private UIFurnaceWindow furnaceWindow;
        [SerializeField] private UIVillageOptionsWindow villageWindow;
        [SerializeField] private UILoreScrollWindow loreScrollWindow;
        [SerializeField] private RenameIslandWindow renameIslandWindow;
        [SerializeField] private UIGetScrollWindow getScrollWindow;
        [SerializeField] private UISoulsShopWindow soulsShopWindow;
        [SerializeField] private UIPlayerInventoryWindow playerInventoryWindow;
        [SerializeField] private UIQuestsOverlay questOverlay;
        [SerializeField] private UICraftPinWidget craftPinWidget;
        [SerializeField] private UIActionsIndicators actionsIndicators;
        [Space, SerializeField] private WindowsManager windowsManager = new WindowsManager();
        
        private PlayerCharacter targetCharacter;
        private IResourcesService resourcesService;
        private GameStateService gameState;
        private TickService tickService;

        public WindowsManager WindowManager => windowsManager;

        [Inject]
        private void Construct(
            SelectionService selectionService,
            TickService tickService,
            IResourcesService resourcesService,
            GameDataObject gameDataObject,
            GameStateService gameState,
            GameStateService gameStateService,
            CharacterService characterService,
            IRaftBuildService raftBuildService, 
            SaveDataObject saveDataObject,
            ScenesService scenesService,
            SaveService saveService,
            PrefabSpawnerFabric spawnerFabric, 
            PlayerAuthService authService,
            CloudService cloudService,
            QuestService questService
        )
        {
            this.tickService = tickService;
            this.gameState = gameState;
            this.resourcesService = resourcesService;


            deathWindow.Init(characterService, saveDataObject, scenesService);
            
            actionManager.Init(selectionService);
            rewindButton.Init(tickService, gameStateService, this);
            mapButton.Init(raftBuildService, scenesService, saveService);
            stopBuildButton.Init(tickService, gameStateService, this);
            exitIslandButton.Init(messageBoxManager, saveService, scenesService);
            playerStorageButton.Init(saveDataObject);
            optionsHolder.Init(storagesCounter, authService, saveService, cloudService, scenesService, messageBoxManager, saveDataObject);
            chestShow.Init(gameDataObject, selectionService);
            craftsWindow.Init(selectionService, gameDataObject, this.resourcesService, this, gameStateService, raftBuildService, saveDataObject);
            craftingTableWindow.Init(selectionService, gameDataObject, this.resourcesService, this, raftBuildService, saveDataObject);
            characterWindow.Init(selectionService, gameDataObject, tickService, raftBuildService, messageBoxManager, spawnerFabric, resourcesService);
            furnaceWindow.Init(selectionService, raftBuildService, tickService, resourcesService);
            renameIslandWindow.Init(saveDataObject);
            getScrollWindow.Init(gameDataObject, resourcesService);
            charactersList?.Init(characterService, tickService, selectionService);
            soulsCounter.Init(saveDataObject);
            soulsShopWindow.Init(gameDataObject, saveDataObject, selectionService);
            playerInventoryWindow
                .With(x => x.Init(resourcesService))
                .With(x => x.SetPlayerStorage(saveDataObject, gameDataObject));

            craftPinWidget.Init(saveDataObject, gameDataObject, this.resourcesService, scenesService);
            questOverlay.Init(questService, scenesService);
            if (actionsIndicators)
            {
                actionsIndicators.Init(characterService, selectionService);
            }
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

            windowsManager.Init(this,tickService, craftsWindow, characterWindow, craftingTableWindow, furnaceWindow, villageWindow, loreScrollWindow, soulsShopWindow, playerInventoryWindow, optionsHolder.Window);
            
            selectionService.OnChangeSelectCharacter += ChangeCharacter;
            resourcesService.OnChangeResources += OnChangeResources;


            ChangeCharacter(selectionService.SelectedCharacter);
            
        }


        [Button]
        public void OpenSoulsWindow()
        {
            soulsShopWindow.ShowWindow();
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
        

        private void LateUpdate()
        {
            actionManager.Update();
            chestShow.Update();

            if (InputService.EscapeDown)
            {
                if (!windowsManager.CloseLast())
                {
                    optionsHolder.OpenWindow();
                }
            }
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
