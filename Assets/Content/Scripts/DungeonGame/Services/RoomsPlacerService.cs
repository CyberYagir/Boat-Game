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
        [SerializeField] private List<RoomGrid> roomPrefabs;


        private List<RoomGrid> spawnedRooms = new List<RoomGrid>();
        private List<Vector3> centers = new List<Vector3>();

        public List<Vector3> RoomsCenters => centers;


        [Inject]
        private void Construct(WorldGridService worldGridService)
        {
            for (int i = 0; i < 10; i++)
            {
                var item = roomPrefabs.GetRandomItem();
                Vector3 pos = Vector3.zero;
                int trys = 0;
                do
                {
                    pos = Vector3Int.CeilToInt(new Vector3(Random.Range(-maxDistance, maxDistance), 0, Random.Range(-maxDistance, maxDistance)));
                    trys++;
                } while (trys < 50 || worldGridService.IsHavePoints(item.GetPoints(pos)));

                if (!worldGridService.IsHavePoints(item.GetPoints(pos)))
                {
                    var spawned = Instantiate(item, transform)
                        .With(x => x.transform.position = pos)
                        .With(x => spawnedRooms.Add(x));
                    RoomsCenters.Add(spawned.GetCenter());
                    worldGridService.AddPoints(item.GetPoints(pos));
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < RoomsCenters.Count; i++)
            {
                Gizmos.DrawSphere(RoomsCenters[i], 0.5f);
            }
        }
    }
}
