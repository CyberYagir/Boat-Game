using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIMapButton : MonoBehaviour
    {
        private IRaftBuildService raftBuildService;
        private ScenesService scenesService;
        private SaveService saveService;

        public void Init(IRaftBuildService raftBuildService, ScenesService scenesService, SaveService saveService)
        {
            this.saveService = saveService;
            this.scenesService = scenesService;
            this.raftBuildService = raftBuildService;
            raftBuildService.OnChangeRaft += RaftBuildServiceOnOnChangeRaft;

            RaftBuildServiceOnOnChangeRaft();
        }

        private void RaftBuildServiceOnOnChangeRaft()
        {
            if (scenesService.GetActiveScene() == ESceneName.IslandGame)
            {
                gameObject.SetActive(false);
                return;
            };
            
            var state = raftBuildService.IsCanMoored();
            gameObject.SetActive(state);

            if (state)
            {
                scenesService.AddScene(ESceneName.Map);
            }
            else
            {
                scenesService.ChangeActiveScene(ESceneName.BoatGame);
                scenesService.UnloadScene(ESceneName.Map);
            }
        }

        public void SwitchScene()
        {
            if (scenesService.GetActiveScene() == ESceneName.Map)
            {
                scenesService.ChangeActiveScene(ESceneName.BoatGame);
            }
            else
            {
                saveService.SaveWorld();
                scenesService.ChangeActiveScene(ESceneName.Map);
            }
        }
    }
}
