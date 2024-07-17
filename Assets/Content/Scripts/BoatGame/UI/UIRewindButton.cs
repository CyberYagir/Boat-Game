using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    [System.Serializable]
    public class UIRewindButton : MonoBehaviour 
    {
        [SerializeField] protected RectTransform button;
        [SerializeField] protected Image image;
        private TickService tickService;
        protected Vector3 startPosition;

        private bool isPressed = false;
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

        public void ButtonDown()
        {
            if (isPressed) return;
            isPressed = true;
            button.DOScale(Vector3.one * 0.8f, 0.2f);
            image.DOFade(0.8f, 0.2f);
            OnButtonDown();
        }

        public virtual void OnButtonDown()
        {
            tickService.ChangeTimeScale(10);
        }
            
            
        public void ButtonUp()
        {
            if (!isPressed) return;
            isPressed = false;
            button.DOScale(Vector3.one, 0.2f);
            image.DOFade(1f, 0.2f);

            OnButtonUp();
        }

        public virtual void OnButtonUp()
        {
            tickService.ChangeTimeScale(1);
        }
    }
}