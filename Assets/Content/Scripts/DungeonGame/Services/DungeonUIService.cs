using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
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
        private DungeonSelectionService selectionService;

        [Inject]
        private void Construct(
            DungeonCharactersService charactersService, 
            DungeonCameraMoveService cameraMoveService, 
            DungeonSelectionService selectionService, 
            DungeonResourcesService dungeonResourcesService,
            VirtualRaftsService raftsService,
            GameDataObject gameData,
            TickService tickService,
            PrefabSpawnerFabric fabric
        )
        {
            this.selectionService = selectionService;

            healthbars.Init(charactersService.GetPlayers(), cameraMoveService.Camera);
            potionsList.Init(dungeonResourcesService, selectionService, charactersService, null);
            resourcesCounter.Init(raftsService, gameData, dungeonResourcesService, tickService);
            storagesCounter.Init(raftsService);
            exitDungeonButton.Init(messageBoxManager);
            characterPreviews.Init(charactersService, fabric, selectionService, this);
            openCharactersButton.Init(this);
            characterWindow.Init(selectionService, gameData, tickService, raftsService, messageBoxManager, fabric, dungeonResourcesService);
            
            characterWindow.OnClose += DeselectCharacter;
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
    }
}
