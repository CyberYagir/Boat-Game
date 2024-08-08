using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class PathfindService : MonoBehaviour
    {
        private WorldGridServiceTyped worldGridService;
        private MSTCalculatorService calculatorService;
        private RoomsPlacerService roomsPlacerService;

        [Inject]
        private void Construct(WorldGridServiceTyped worldGridService, RoomsPlacerService roomsPlacerService, MSTCalculatorService calculatorService)
        {
            this.roomsPlacerService = roomsPlacerService;
            this.calculatorService = calculatorService;
            this.worldGridService = worldGridService;

            PathfindHallways();
        }
        
        void PathfindHallways()
        {
            DungeonPathfinder2D aStar = new DungeonPathfinder2D(roomsPlacerService.Size);

            var selectedEdges = calculatorService.GetSelectedEdges();
                
            foreach (var edge in selectedEdges)
            {
                
                
                var startRoom =roomsPlacerService.FindRoomByNode(edge.NodeA);
                var endRoom = roomsPlacerService.FindRoomByNode(edge.NodeB);

                var startPosf = startRoom.GetCenter();
                var endPosf = endRoom.GetCenter();
                var startPos = new Vector2Int((int) startPosf.x, (int) startPosf.z);
                var endPos = new Vector2Int((int) endPosf.x, (int) endPosf.z);

                var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) =>
                {
                    var pathCost = new DungeonPathfinder2D.PathCost();

                    pathCost.cost = Vector2Int.Distance(b.Position, endPos); //heuristic

                    worldGridService.IsHavePoint(new Vector3(b.Position.x, 0, b.Position.y), out var type);

                    switch (type)
                    {
                        case WorldGridServiceTyped.ECellType.Filled:
                            pathCost.cost += 5;
                            break;
                        case WorldGridServiceTyped.ECellType.Room:
                            pathCost.cost += 10;
                            break;
                    }

                    pathCost.traversable = true;

                    return pathCost;
                });

                if (path != null)
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        var current = path[i];

                        worldGridService.AddPoint(new Vector3(current.x, 0, current.y), WorldGridServiceTyped.ECellType.Filled);
                    }

                    // foreach (var pos in path)
                    // {
                    //     if (grid[pos] == CellType.Hallway)
                    //     {
                    //         PlaceHallway(pos);
                    //     }
                    // }
                }
            }
        }
    }
}
