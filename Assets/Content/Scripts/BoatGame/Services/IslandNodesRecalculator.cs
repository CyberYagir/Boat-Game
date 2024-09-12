using System;
using Pathfinding;
using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public class IslandNodesRecalculator : MonoBehaviour
    {
        [SerializeField] private NavMeshService navMeshService;
        private bool isTerrainGraphRecalculated = false;
        
        private void Awake()
        {
            navMeshService.OnNavMeshBuild += NavMeshServiceOnOnNavMeshBuild;
        }

        private void NavMeshServiceOnOnNavMeshBuild()
        {
            if (AstarPath.active.data.graphs[0].isScanned && !isTerrainGraphRecalculated)
            {
                var playerNode = AstarPath.active.GetNearest(Vector3.zero, NNConstraint.Walkable).node;
                AstarPath.active.AddWorkItem(() =>
                {
                    AstarPath.active.data.gridGraph.GetNodes(node =>
                    {
                        if (node.Area != playerNode.Area) node.Walkable = false;
                    });
                });
                print("Recalculate");
                isTerrainGraphRecalculated = true;
            }
        }
    }
}
