using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonTileGenerationService : MonoBehaviour
    {
        [SerializeField] private DungeonTile tile;

        private List<Vector3> tmpDirs = new List<Vector3>();
        private WorldGridServiceTyped worldGridService;

        [Inject]
        private void Construct(WorldGridServiceTyped worldGridService, RoomsPlacerService roomsPlacerService)
        {
            this.worldGridService = worldGridService;
            WorldGridServiceTyped.ECellType tileType;
            foreach (var point in worldGridService.GetAllPoints())
            {
                worldGridService.IsHavePoint(point, out tileType);
                Instantiate(tile, point, Quaternion.identity, transform)
                    .With(x => x.Init(GetDirectionsData(point), tileType));
            }
            
            

            foreach (var rooms in roomsPlacerService.SpawnedRooms)
            {
                rooms.CheckAllTiles(worldGridService);
                rooms.transform.parent = transform;
            }

            transform.localScale = Vector3.one * 3f;
        }

        private List<Vector3> GetDirectionsData(Vector3 point)
        {
            tmpDirs.Clear();
            
            CheckDirection(point, Vector3Int.forward);
            CheckDirection(point, Vector3Int.back);
            CheckDirection(point, Vector3Int.left);
            CheckDirection(point, Vector3Int.right);

            return tmpDirs;
        }

        private void CheckDirection(Vector3 point, Vector3 dir)
        {
            if (worldGridService.IsHavePoint(point + dir))
            {
                tmpDirs.Add(dir);
            }
        }
    }
}
