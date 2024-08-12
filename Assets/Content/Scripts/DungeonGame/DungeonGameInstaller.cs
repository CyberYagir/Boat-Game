using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Game;

namespace Content.Scripts.DungeonGame
{
    public class DungeonGameInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            Container.Bind<PrefabSpawnerFabric>().AsSingle().NonLazy();
            BindService<DungeonService>();
            BindService<WorldGridServiceTyped>();
            BindService<RoomsPlacerService>();
            BindService<TriangulationService>();
            BindService<MSTCalculatorService>();
            BindService<PathfindService>();
            BindService<INavMeshProvider>();
            BindService<DungeonCharactersService>();
            BindService<DungeonTileGenerationService>();
            BindService<UrnCollectionService>();
            BindService<DungeonSelectionService>();
            BindService<DungeonCameraMoveService>();
            BindService<VirtualRaftsService>();
            BindService<DungeonResourcesService>();
            BindService<MobsSpawnService>();
            BindService<DungeonEnemiesService>();
        }
    }
}