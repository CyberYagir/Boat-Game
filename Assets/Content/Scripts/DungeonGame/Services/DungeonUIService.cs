using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.UI;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonUIService : MonoBehaviour
    {
        [SerializeField] private UIHealthbars healthbars;
        [SerializeField] private UIPotionsList potionsList;
        
        [Inject]
        private void Construct(DungeonCharactersService charactersService, DungeonCameraMoveService cameraMoveService, DungeonSelectionService selectionService, DungeonResourcesService dungeonResourcesService)
        {
            healthbars.Init(charactersService.GetPlayers(), cameraMoveService.Camera);
            potionsList.Init(dungeonResourcesService, selectionService, charactersService, null);
        }


        private void LateUpdate()
        {
            healthbars.UpdateBars();
        }
    }
}
