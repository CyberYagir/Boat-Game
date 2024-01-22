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
        
        public virtual void Init(TickService tickService, GameStateService gameStateService)
        {
            this.tickService = tickService;
            startPosition = button.anchoredPosition;
            
            gameStateService.OnChangeEState += GameStateServiceOnOnChangeEState;
        }

        public virtual void GameStateServiceOnOnChangeEState(GameStateService.EGameState mewState)
        {
            if (mewState == GameStateService.EGameState.Building)
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