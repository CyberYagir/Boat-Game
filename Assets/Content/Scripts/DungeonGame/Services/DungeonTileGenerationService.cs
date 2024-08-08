using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonTileGenerationService : MonoBehaviour
    {
        [SerializeField] private DungeonTile tile;
        [SerializeField] private NoneTile noneTile;

        private List<Vector3> tmpDirs = new List<Vector3>();
        private WorldGridServiceTyped worldGridService;

        [Inject]
        private void Construct(
            WorldGridServiceTyped worldGridService, 
            RoomsPlacerService roomsPlacerService, 
            INavMeshProvider navMeshProvider, 
            PrefabSpawnerFabric fabric, 
            DungeonCharactersService dungeonCharactersService)
        {
            this.worldGridService = worldGridService;
            WorldGridServiceTyped.ECellType tileType;
            foreach (var point in worldGridService.GetAllPoints())
            {
                worldGridService.IsHavePoint(point, out tileType);
                var obj = Instantiate(tile, point, Quaternion.identity, transform)
                    .With(x => x.Init(GetDirectionsData(point), tileType));
                
                fabric.InjectComponent(obj.gameObject);
            }
            

            foreach (var rooms in roomsPlacerService.SpawnedRooms)
            {
                rooms.CheckAllTiles(worldGridService);
                rooms.transform.parent = transform;
                fabric.InjectComponent(rooms.gameObject);
            }

            for (int i = -5; i < roomsPlacerService.Size.x + 5; i++)
            {
                for (int j = -5; j < roomsPlacerService.Size.y + 5; j++)
                {
                    var pos = new Vector3(i, 0, j);
                    if (!worldGridService.IsHavePoint(pos))
                    {
                        var obj = Instantiate(noneTile, pos, Quaternion.identity, transform)
                            .With(x=>x.Init());
                        fabric.InjectComponent(obj.gameObject);
                    }
                }
            }

            transform.localScale = Vector3.one * 3f;
            transform.Translate(Vector3.up * 3f);
            
            navMeshProvider.BuildNavMesh();
            
            foreach (var spawned in dungeonCharactersService.SpawnedCharacters)
            {
                spawned.SetPosition(roomsPlacerService.GetStartRoomRandomPos());
            }
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
