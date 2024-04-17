using Pathfinding;

namespace Content.Scripts.BoatGame.Services
{
    public interface INavMeshProvider
    {
        public void BuildNavMesh();
        public void BuildNavMeshAsync();

        public void BuildNavMeshAsync(int id);
        NavGraph GetNavMeshByID(int i);
        int GetGraphsCount();
    }
}