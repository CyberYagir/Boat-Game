using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIExitIslandButton : MonoBehaviour
    {
        private UIMessageBoxManager messageBoxManager;
        private ScenesService scenesService;
        private SaveService saveService;

        public void Init(UIMessageBoxManager messageBoxManager, SaveService saveService, ScenesService scenesService, GameStateService gameStateService)
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.messageBoxManager = messageBoxManager;

            gameObject.SetActive(scenesService.GetActiveScene() == ESceneName.IslandGame);
            
            
            gameStateService.OnChangeEState += OnChangeGameState;
        }

        private void OnChangeGameState(GameStateService.EGameState obj)
        {


            if (obj == GameStateService.EGameState.Normal && scenesService.GetActiveScene() == ESceneName.IslandGame)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void ButtonClick()
        {
            messageBoxManager.ShowMessageBox("Do you want to leave this island?", delegate
            {
                saveService.SaveWorld();
                saveService.ExitFromIsland();
                scenesService.FadeScene(ESceneName.BoatGame);
            });
        }
    }
}
