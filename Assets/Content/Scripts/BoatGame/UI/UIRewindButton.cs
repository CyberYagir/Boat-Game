using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class UIRewindButton : UIBigButtonBase 
    {
        private TickService tickService;
        protected Vector3 startPosition;

        private bool isSpaceHold = false;
        private UIService uiService;

        public virtual void Init(TickService tickService, GameStateService gameStateService, UIService uiService)
        {
            this.uiService = uiService;
            this.tickService = tickService;
            startPosition = button.anchoredPosition;
            
            gameStateService.OnChangeEState += GameStateServiceOnOnChangeEState;
            
            tickService.OnTick += OnTick;
        }

        private void OnTick(float obj)
        {
            if (InputService.SpaceHold)
            {
                if (!isPressed && !isSpaceHold && !uiService.WindowManager.isAnyWindowOpened)
                {
                    ButtonDown();
                    isSpaceHold = true;
                }
            }
            else
            {
                if (isPressed && isSpaceHold || uiService.WindowManager.isAnyWindowOpened)
                {
                    ButtonUp();
                    isSpaceHold = false;
                }
            }
        }

        public virtual void GameStateServiceOnOnChangeEState(GameStateService.EGameState mewState)
        {
            if (mewState == GameStateService.EGameState.Building || mewState == GameStateService.EGameState.Removing)
            {
                button.DOAnchorPos(startPosition + Vector3.down * 1000, 0.2f);
            }
            else
            {
                button.DOAnchorPos(startPosition, 0.2f);
            }
        }

        public override void OnButtonDown()
        {
            base.OnButtonDown();
            tickService.ChangeTimeScale(10);
        }

        public override void OnButtonUp()
        {
            base.OnButtonUp();
            tickService.ChangeTimeScale(1);
        }
    }
}