using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame.Services
{
    public class RoomsPlacerService : MonoBehaviour
    {
        [SerializeField] private float maxDistance;
        [SerializeField] private int roomsCount = 10;
        [SerializeField] private List<RoomGrid> roomPrefabs;
        [SerializeField] private RoomGrid roomStart, roomEnd;
 
        private List<RoomGrid> spawnedRooms = new List<RoomGrid>();
        
        
        private List<RoomGrid> tmpRoomsList = new List<RoomGrid>();
        
        private List<Vector3> centers = new List<Vector3>();
        
        private RoomGrid spawnedStart;
        private RoomGrid spawnedEnd;

        public List<Vector3> RoomsCenters => centers;

        public Vector2Int Size => new Vector2Int((int)maxDistance, (int)maxDistance) + Vector2Int.one;

        public List<RoomGrid> SpawnedRooms => spawnedRooms;


        [Inject]
        private void Construct(WorldGridServiceTyped worldGridService, DungeonService dungeonService)
        {
            var rnd = dungeonService.TargetRnd;


            spawnedEnd = SpawnRoom(worldGridService, roomEnd, new Vector3(maxDistance - roomStart.Size.x - 1, 0, maxDistance - roomStart.Size.y - 1));
            spawnedStart = SpawnRoom(worldGridService, roomStart, new Vector3(0 + roomStart.Size.x, 0, 0 + roomStart.Size.y));


            for (int i = 0; i < roomsCount; i++)
            {
                if (tmpRoomsList.Count == 0)
                {
                    tmpRoomsList.AddRange(roomPrefabs);
                    tmpRoomsList.Shuffle(rnd);
                }

                var item = tmpRoomsList.GetRandomItem(rnd);
                tmpRoomsList.Remove(item);
                Vector3 pos = Vector3.zero;
                int trys = 0;
                do
                {
                    pos = Vector3Int.CeilToInt(new Vector3(rnd.NextFloat(-maxDistance, maxDistance), 0, rnd.NextFloat(-maxDistance, maxDistance)));
                    trys++;
                } while (trys < 50 || worldGridService.IsHavePoints(item.GetPointsInGlobalSpace(pos)) || !InBounds(item.GetPointsInGlobalSpace(pos)));

                SpawnRoom(worldGridService, item, pos);
            }
        }

        private RoomGrid SpawnRoom(WorldGridServiceTyped worldGridService, RoomGrid item, Vector3 pos)
        {
            if (!worldGridService.IsHavePoints(item.GetPointsInGlobalSpace(pos)))
            {
                var spawned = Instantiate(item, transform)
                    .With(x => x.transform.position = pos)
                    .With(x => SpawnedRooms.Add(x));
                RoomsCenters.Add(spawned.GetCenter());
                worldGridService.AddPoints(item.GetPointsInGlobalSpace(pos), WorldGridServiceTyped.ECellType.Room);

                return spawned;
            }

            return null;
        }

        private bool InBounds(List<Vector3> getPoints)
        {
            foreach (var c in getPoints)
            {
                if (c.x < 0 || c.y < 0 || c.z < 0)
                {
                    return false;
                }
                if (c.x >= maxDistance || c.y >= maxDistance || c.z >= maxDistance)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            return;
            Gizmos.color = Color.red;
            for (int i = 0; i < RoomsCenters.Count; i++)
            {
                Gizmos.DrawSphere(RoomsCenters[i], 0.5f);
            }
        }

        public RoomGrid FindRoomByNode(MSTCalculatorService.Node edgeNodeA)
        {
            return SpawnedRooms.Find(x=>Vector2Int.RoundToInt(new Vector2(x.GetCenter().x, x.GetCenter().z)) == edgeNodeA.Pos);
        }

        public Vector3 GetStartRoomRandomPos()
        {
            return spawnedStart.GetPointsInGlobalSpace(spawnedStart.transform.position).GetRandomItem();
        }
    }
}
