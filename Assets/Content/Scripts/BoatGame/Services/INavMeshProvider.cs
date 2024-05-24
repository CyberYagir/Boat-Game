using System;
using Pathfinding;

namespace Content.Scripts.BoatGame.Services
{
    public interface INavMeshProvider
    {
        public event Action OnNavMeshBuild;
        public void BuildNavMesh();
        public void BuildNavMeshAsync();

        public void BuildNavMeshAsync(int id);
        NavGraph GetNavMeshByID(int i);
        int GetGraphsCount();
    }
}