using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class NativeWorldIcon : MonoBehaviour
    {
        [SerializeField] private Transform part;
        [SerializeField] private Transform point;
        [SerializeField] private GraphicRaycaster canvas;
        [SerializeField] private EStateType stateType = EStateType.VillageViewInfo;
        private Camera mainCamera;
        private SelectionService selectionService;
        private GameStateService gameStateService;

        [Inject]
        private void Construct(SelectionService selectionService, GameStateService gameStateService)
        {
            this.gameStateService = gameStateService;
            this.selectionService = selectionService;
            mainCamera = selectionService.Camera;
            
            selectionService.AddRaycaster(canvas);

        }
        

        private void LateUpdate()
        {
            transform.position = mainCamera.WorldToScreenPoint(point.transform.position);
            
            
            if (selectionService.SelectedCharacter != null && selectionService.SelectedCharacter.CurrentState == stateType || gameStateService.GameState != GameStateService.EGameState.Normal)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.unscaledDeltaTime * 5f);
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.unscaledDeltaTime * 5f);
            }
        }

        public void ActiveAction()
        {
            if (gameStateService.GameState != GameStateService.EGameState.Normal) return;
            
            
            var action = GetComponentInParent<ISelectable>();
            selectionService.SelectObject(action, false);
            action.PlayerActions[0].Action();
            selectionService.ClearSelectedObject();
        }
    }
}
