using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Services;
using Pathfinding;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    public class IslandTransferRaftService : MonoBehaviour
    {
        [SerializeField] private Transform raftsSpawnPoint;
        [SerializeField] private RaftLadderToIsland ladderPrefab;
        [SerializeField] private DirectionalTrigger directionalTriggerPrefab;



        private DirectionalTrigger enterRaftTrigger, exitRaftTrigger;

        private RaftBuildService raftBuildService;
        private PrefabSpawnerFabric prefabSpawnerFabric;
        
        public event Action OnRaftTransferingEnding;

        public DirectionalTrigger EnterRaftTrigger => enterRaftTrigger;

        public DirectionalTrigger ExitRaftTrigger => exitRaftTrigger;

        public Vector3 RaftPoint => raftPoint;
        public Vector3 EndPoint => endPoint;

        private Vector3 raftPoint;
        private Vector3 endPoint;
        

        [Inject]
        public void Construct(IslandGenerator islandGenerator, RaftBuildService raftBuildService, CameraMovingService cameraMovingService, PrefabSpawnerFabric prefabSpawnerFabric)
        {
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.raftBuildService = raftBuildService;
            Random rnd = new Random(islandGenerator.Seed);

            var spawnPoint = islandGenerator.CurrentIslandData.SpawnPoints.GetRandomItem(rnd);

            Physics.Raycast(spawnPoint.LadderPoint.position + Vector3.up * 100, Vector3.down, out RaycastHit hit);
            spawnPoint.LadderPoint.position = hit.point;
            spawnPoint.Point.position = new Vector3(spawnPoint.Point.position.x, 0, spawnPoint.Point.position.z);

            raftsSpawnPoint.transform.position = spawnPoint.Point.position;


            cameraMovingService.SetStartPosition(spawnPoint.Point.position);

            StartCoroutine(WaitFrameToBuildNavMesh(spawnPoint));

        }

        IEnumerator WaitFrameToBuildNavMesh(IslandData.SpawnPoint spawnPoint)
        {
            yield return null;

            var minDist = 99999f;
            var id = -1;
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                var dst = Vector3.Distance(spawnPoint.Point.position, raftBuildService.SpawnedRafts[i].transform.position);
                if (dst < minDist)
                {
                    minDist = dst;
                    id = i;
                }
            }

            raftPoint = raftBuildService.SpawnedRafts[id].transform.position;
            endPoint = spawnPoint.LadderPoint.position;
            
            prefabSpawnerFabric.SpawnItem(ladderPrefab)
                .With(x => x.Init(raftBuildService.SpawnedRafts[id].transform.position, spawnPoint.LadderPoint.position));


            exitRaftTrigger = prefabSpawnerFabric.SpawnItem(directionalTriggerPrefab, spawnPoint.LadderPoint.position + Vector3.up, Quaternion.LookRotation(spawnPoint.LadderPoint.position - spawnPoint.Point.position))
                .With(x => x.transform.eulerAngles = new Vector3(0, x.transform.eulerAngles.y, 0))
                .With(x => x.transform.name = " exit trigger");

            enterRaftTrigger = prefabSpawnerFabric.SpawnItem(directionalTriggerPrefab, spawnPoint.LadderPoint.position + Vector3.up, Quaternion.LookRotation(spawnPoint.Point.position - spawnPoint.LadderPoint.position))
                .With(x => x.transform.eulerAngles = new Vector3(0, x.transform.eulerAngles.y, 0))
                .With(x=>x.transform.position += x.transform.forward * 2.5f)
                .With(x => x.transform.name = " enter trigger");

            OnRaftTransferingEnding?.Invoke();
        }
    }
}
