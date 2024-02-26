using Content.Scripts.BoatGame;

namespace Content.Scripts.IslandGame
{
    public class IslandGameInstaller : GameInstaller
    {
        public override void InstallBindings()
        {
            InstallIslandGame();
            InstallBoatGame();
        }
    }
}
