using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIStopBuildButton : UIRewindButton
    {
        private GameStateService gameStateService;

        public override void Init(TickService tickService, GameStateService gameStateService, UIService uiService)
        {
            this.gameStateService = gameStateService;
            base.Init(tickService, gameStateService, uiService);
            button.anchoredPosition = startPosition + Vector3.down * 1000;
        }

        public override void GameStateServiceOnOnChangeEState(GameStateService.EGameState mewState)
        {
            if (mewState is GameStateService.EGameState.Building or GameStateService.EGameState.Removing or GameStateService.EGameState.BuildingStructures)
            {
                gameObject.SetActive(true);
                button.DOAnchorPos(startPosition, 0.2f);
            }
            else
            {
                button.DOScale(Vector3.one, 0.2f);
                image.DOFade(1f, 0.2f);
                button.DOAnchorPos(startPosition + Vector3.down * 1000, 0.2f).onComplete += delegate
                {
                    gameObject.SetActive(false);
                };
            }
        }

        public override void OnButtonDown()
        {
        }

        public override void OnButtonUp()
        {
            gameStateService.ChangeGameState(GameStateService.EGameState.Normal);
        }
    }
}
