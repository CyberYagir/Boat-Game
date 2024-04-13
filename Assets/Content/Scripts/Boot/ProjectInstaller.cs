using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;

namespace Content.Scripts.Boot
{
    public class ProjectInstaller : MonoBinder
    {
        private ScenesService scenesService;
        public override void InstallBindings()
        {
            BindService<ScenesService>();

            scenesService = Container.Resolve<ScenesService>();
        }

        private void Update()
        {
            TimeService.AddPlayedTime();
            if (scenesService.GetActiveScene() is ESceneName.BoatGame or ESceneName.Map)
            {
                TimeService.AddPlayedBoatTime();
            }
        }
    }
}