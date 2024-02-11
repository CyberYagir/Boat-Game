using Content.Scripts.Game;

namespace Content.Scripts.Map
{
    public class MapInstaller : MonoBinder
    {
        public override void InstallBindings()
        {
            BindService<MapSpawnerService>();
            BindService<MapMoverService>();
            BindService<MapIslandCollector>();
            BindService<MapSelectionService>();
        }
    }
}