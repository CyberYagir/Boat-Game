using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Services;

namespace Content.Scripts.BoatGame
{
    public class GameInstaller : MonoBinder
    {
        public void InstallIslandGame()
        {
            BindService<CameraMovingService>();
            BindService<IslandGenerator>();
            BindService<IslandTransferRaftService>();
        }
        public void InstallBoatGame()
        {
            Container.Bind<PrefabSpawnerFabric>().AsSingle().NonLazy();
            
            BindService<GameStateService>();
            BindService<TickService>();
            BindService<INavMeshProvider>();
            BindService<WorldGridService>();
            BindService<WeatherService>();
            BindService<RaftBuildService>();
            BindService<RaftDamagerService>();
            BindService<ResourcesService>();
            BindService<SelectionService>();
            BindService<CharacterService>();
            BindService<SaveService>();
        }
    }
}