using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Services;
using Content.Scripts.QuestsSystem;

namespace Content.Scripts.BoatGame
{
    public class GameInstaller : MonoBinder
    {
        public void InstallIslandGame()
        {
            BindService<CameraMovingService>();
            BindService<IslandMobsService>();
            BindService<IslandGenerator>();
            BindService<IslandTransferRaftService>();
        }
        public void InstallBoatGame()
        {
            Container.Bind<PrefabSpawnerFabric>().AsSingle().NonLazy();
            BindService<WorldPopupService>();
            
            BindService<GameStateService>();
            BindService<TickService>();
            BindService<INavMeshProvider>();
            BindService<WorldGridService>();
            BindService<WeatherService>();
            BindService<IRaftBuildService>();
            BindService<RaftDamagerService>();
            BindService<IResourcesService>();
            BindService<SelectionService>();
            BindService<CharacterService>();
            Container.Bind<QuestBuilder>().AsSingle().NonLazy();
            BindService<QuestService>();
            BindService<SaveService>();
        }
    }
}