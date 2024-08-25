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
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        [SerializeField] private UISelectCharacterWindow characterPreviews;
        [SerializeField] private UICharacterWindow characterWindow;
        [SerializeField] private UIDeathWindow deathWindow;
        [SerializeField] private UIDungeonMobsCounter mobsCounter;
        
        private DungeonSelectionService selectionService;
        private SaveDataObject saveDataObject;
        private ScenesService scenesService;
        private DungeonSaveService saveService;

        [Inject]
        private void Construct(
            DungeonCharactersService charactersService, 
            DungeonCameraMoveService cameraMoveService, 
            DungeonSelectionService selectionService, 
            DungeonResourcesService dungeonResourcesService,
            VirtualRaftsService raftsService,
            GameDataObject gameData,
            TickService tickService,
            PrefabSpawnerFabric fabric,
            ScenesService scenesService,
            SaveDataObject saveDataObject, 
            DungeonSaveService saveService,
            DungeonEnemiesService enemiesService,
            DungeonService dungeonService,
            RoomsPlacerService roomsPlacerService
        )
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.saveDataObject = saveDataObject;
            this.selectionService = selectionService;

            healthbars.Init(charactersService.GetPlayers(), cameraMoveService.Camera);
            potionsList.Init(dungeonResourcesService, selectionService, charactersService, null, scenesService);
            resourcesCounter.Init(raftsService, gameData, dungeonResourcesService, tickService);
            storagesCounter.Init(raftsService);
            exitDungeonButton.Init(messageBoxManager, this, charactersService);
            characterPreviews.Init(charactersService, fabric, selectionService, this);
            openCharactersButton.Init(this, charactersService);
            characterWindow.Init(selectionService, gameData, tickService, raftsService, messageBoxManager, fabric, dungeonResourcesService);
            deathWindow.Init(charactersService, saveDataObject, this.scenesService);
            mobsCounter.Init(enemiesService, dungeonService);
            
            roomsPlacerService.SpawnedEnd.GetComponent<DungeonRoomEnd>().OnEnter += OnEnterBoss;
            characterWindow.OnClose += DeselectCharacter;
        }

        private void OnEnterBoss()
        {
            messageBoxManager.ShowMessageBox("Do you want to enter the boss?", delegate
            {
                
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
