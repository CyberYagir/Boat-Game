using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.Boot;
using Content.Scripts.DungeonGame.UI;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonUIService : MonoBehaviour
    {
        [SerializeField] private UIHealthbars healthbars;
        [SerializeField] private UIPotionsList potionsList;
        [SerializeField] private UIResourcesCounter resourcesCounter;
        [SerializeField] private UIStoragesCounter storagesCounter;
        [SerializeField] private UIExitDungeonButton exitDungeonButton;
        [SerializeField] private UICharactersOpenButton openCharactersButton;
        [SerializeField] private UIOptionsHolder optionsHolder;
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private UISelectCharacterWindow characterPreviews;
        [SerializeField] private UICharacterWindow characterWindow;
        [SerializeField] private UIDeathWindow deathWindow;
        [SerializeField] private UIDungeonMobsCounter mobsCounter;
        [SerializeField] private UIDungeonMobsCompass dungeonCompass;
        [SerializeField] private UIGetScrollWindow getScrollWindow;
        [SerializeField] private UIBossHealthDisplay bossHealthDisplay;
        [SerializeField] private UISoulsCounter soulsCounter;
        [SerializeField] private UIQuestsOverlay questsOverlay;
        private DungeonSelectionService selectionService;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;
        private DungeonSaveService saveService;

        [Inject]
        private void Construct(
            DungeonCharactersService charactersService, 
            DungeonCameraMoveService cameraMoveService, 
            DungeonSelectionService selectionService, 
            IResourcesService dungeonResourcesService,
            IRaftBuildService raftsService,
            GameDataObject gameData,
            TickService tickService,
            PrefabSpawnerFabric fabric,
            ScenesService scenesService,
            SaveDataObject saveDataObject, 
            DungeonSaveService saveService,
            DungeonEnemiesService enemiesService,
            DungeonService dungeonService,
            RoomsPlacerService roomsPlacerService,
            PlayerAuthService authService,
            CloudService cloudService,
            QuestService questService
        )
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.saveDataObject = saveDataObject;
            this.selectionService = selectionService;
            var endRoom = roomsPlacerService.SpawnedEnd.GetComponent<DungeonRoomEnd>();

            healthbars.Init(charactersService.GetPlayers(), cameraMoveService.Camera);
            potionsList.Init(dungeonResourcesService, selectionService, charactersService, null, scenesService);
            resourcesCounter.Init(raftsService, gameData, dungeonResourcesService, tickService);
            storagesCounter.Init(raftsService);
            exitDungeonButton.Init(messageBoxManager, this, charactersService, enemiesService);
            characterPreviews.Init(charactersService, fabric, selectionService, this);
            openCharactersButton.Init(this, charactersService);
            characterWindow.Init(selectionService, gameData, tickService, raftsService, messageBoxManager, fabric, dungeonResourcesService);
            deathWindow.Init(charactersService, saveDataObject, this.scenesService);
            mobsCounter.Init(enemiesService, dungeonService);
            optionsHolder.Init(storagesCounter, authService, this.saveService, cloudService, scenesService, messageBoxManager, saveDataObject);
            dungeonCompass.Init(enemiesService, charactersService);
            getScrollWindow.Init(gameData, dungeonResourcesService);
            bossHealthDisplay.Init(enemiesService);
            soulsCounter.Init(saveDataObject);
            questsOverlay.Init(questService);
            
            endRoom.OnEnter += OnEnterBoss;
            
            characterWindow.OnClose += DeselectCharacter;
        }

        private void OnEnterBoss(DungeonRoomEnd obj)
        {
            messageBoxManager.ShowMessageBox("Do you want to enter the boss?", delegate
            {
                obj.EnterBoss();
            });
        }


        private void DeselectCharacter(AnimatedWindow obj)
        {
            selectionService.SetActiveCharacter(null);
        }


        private void LateUpdate()
        {
            healthbars.UpdateBars();
        }

        public void OpenCharactersPreview()
        {
            characterPreviews.ShowWindow();
        }

        public void ShowCharactersWindow()
        {
            characterPreviews.CloseWindow();
            characterWindow.ShowWindow();
        }

        public void ExitDungeon()
        {
            saveDataObject.Global.ExitDungeon();
            saveService.SaveWorld();
            
            scenesService.FadeScene(ESceneName.Loading);
        }
    }
}
