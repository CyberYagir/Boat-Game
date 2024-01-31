using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;

namespace Content.Scripts.Boot
{
    public class ProjectInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<ScenesService>();
        }

        private void Update()
        {
            TimeService.AddPlayedTime();
        }
    }
}