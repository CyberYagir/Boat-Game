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
        [SerializeField] private UIMessageBoxManager messageBoxManager;
        
        
        [Inject]
        private void Construct(
            DungeonCharactersService charactersService, 
            DungeonCameraMoveService cameraMoveService, 
            DungeonSelectionService selectionService, 
            DungeonResourcesService dungeonResourcesService,
            VirtualRaftsService raftsService,
            GameDataObject gameData,
            TickService tickService
        )
        {
            
            healthbars.Init(charactersService.GetPlayers(), cameraMoveService.Camera);
            potionsList.Init(dungeonResourcesService, selectionService, charactersService, null);
            resourcesCounter.Init(raftsService, gameData, dungeonResourcesService, tickService);
            storagesCounter.Init(raftsService);
            exitDungeonButton.Init(messageBoxManager);
        }


        private void LateUpdate()
        {
            healthbars.UpdateBars();
        }
    }
}
