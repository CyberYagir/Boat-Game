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

        public void Init(UIMessageBoxManager messageBoxManager, SaveService saveService, ScenesService scenesService)
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.messageBoxManager = messageBoxManager;

            gameObject.SetActive(scenesService.GetActiveScene() == ESceneName.IslandGame);
        }
        
        public void ButtonClick()
        {
            messageBoxManager.ShowMessageBox("Do you want to leave this island?", delegate
            {
                saveService.SaveWorld();
                scenesService.FadeScene(ESceneName.BoatGame);
            });
        }
    }
}
