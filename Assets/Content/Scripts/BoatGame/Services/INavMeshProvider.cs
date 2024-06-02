using System;
using Pathfinding;
using UnityEngine;

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
        bool IsAvailablePoint(Vector3 pos);
    }
}