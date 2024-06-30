using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Game;

namespace Content.Scripts.DungeonGame
{
    public class DungeonGameInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<WorldGridService>();
            BindService<RoomsPlacerService>();
        }
    }
}