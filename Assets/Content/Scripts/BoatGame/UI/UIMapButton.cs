using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UIMapButton : MonoBehaviour
    {
        private RaftBuildService raftBuildService;
        private ScenesService scenesService;

        public void Init(RaftBuildService raftBuildService, ScenesService scenesService)
        {
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
                scenesService.ChangeActiveScene(ESceneName.Map);
            }
        }
    }
}
