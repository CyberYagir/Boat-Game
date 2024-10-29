using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Game;
using Content.Scripts.QuestsSystem;

namespace Content.Scripts.DungeonGame
{
    public class DungeonGameInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            Container.Bind<PrefabSpawnerFabric>().AsSingle().NonLazy();
            BindService<TickService>();
            BindService<DungeonService>();
            BindService<WorldGridServiceTyped>();
            BindService<RoomsPlacerService>();
            BindService<TriangulationService>();
            BindService<MSTCalculatorService>();
            BindService<PathfindService>();
            BindService<INavMeshProvider>();
            BindService<DungeonCharactersService>();
            BindService<DungeonTileGenerationService>();
            BindService<DropCollectionService>();
            BindService<UrnCollectionService>();
            BindService<DungeonSelectionService>();
            BindService<DungeonCameraMoveService>();
            BindService<IRaftBuildService>();
            BindService<IResourcesService>();
            BindService<MobsSpawnService>();
            BindService<DungeonEnemiesService>();
            BindService<DungeonSaveService>();
            Container.Bind<QuestBuilder>().AsSingle().NonLazy();
            BindService<QuestService>();
        }
    }
}