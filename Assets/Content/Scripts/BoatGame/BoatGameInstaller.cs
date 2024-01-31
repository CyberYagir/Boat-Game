using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;

namespace Content.Scripts.BoatGame
{
    public class BoatGameInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<GameStateService>();
            BindService<TickService>();
            BindService<WorldGridService>();
            BindService<WeatherService>();
            BindService<RaftBuildService>();
            BindService<RaftDamagerService>();
            BindService<ResourcesService>();
            BindService<SelectionService>();
            BindService<CharacterService>();
        }
    }
}
