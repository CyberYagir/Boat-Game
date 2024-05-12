using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame.Services
{
    public class IslandTransferRaftService : MonoBehaviour
    {
        [SerializeField] private Transform raftsSpawnPoint;
        [SerializeField] private RaftLadderToIsland ladderPrefab;



        private DirectionalTrigger enterRaftTrigger, exitRaftTrigger;

        private RaftBuildService raftBuildService;
        private PrefabSpawnerFabric prefabSpawnerFabric;
        
        public event Action OnRaftTransferingEnding;

        public Vector3 RaftPoint => raftPoint;
        public Vector3 EndPoint => endPoint;

        private Vector3 raftPoint;
        private Vector3 endPoint;
        private CharacterService characterService;


        [Inject]
        public void Construct(
            IslandGenerator islandGenerator, 
            RaftBuildService raftBuildService, 
            CameraMovingService cameraMovingService, 
            PrefabSpawnerFabric prefabSpawnerFabric, 
            CharacterService characterService)
        {
            this.characterService = characterService;
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.raftBuildService = raftBuildService;
            
            Random rnd = new Random(islandGenerator.Seed);

            var spawnPoint = islandGenerator.CurrentIslandData.SpawnPoints.GetRandomItem(rnd);

            // Physics.Raycast(spawnPoint.LadderPoint.position + Vector3.up * 100, Vector3.down, out RaycastHit hit);
            // var point = hit.point;
            // point.y = 0;
            // spawnPoint.LadderPoint.position = point;
            // spawnPoint.Point.position = new Vector3(spawnPoint.Point.position.x, 0, spawnPoint.Point.position.z);

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
                if (raftBuildService.SpawnedRafts[i].IsWalkableRaft)
                {
                    var dst = Vector3.Distance(spawnPoint.LadderPoint.position, raftBuildService.SpawnedRafts[i].transform.position);
                    if (dst < minDist)
                    {
                        minDist = dst;
                        id = i;
                    }
                }
            }

            raftPoint = raftBuildService.SpawnedRafts[id].transform.position;
            endPoint = spawnPoint.LadderPoint.position;
            
            prefabSpawnerFabric.SpawnItem(ladderPrefab)
                .With(x => x.Init(raftBuildService.SpawnedRafts[id].transform.position, spawnPoint.LadderPoint.position));
            
            
            foreach (var character in characterService.SpawnedCharacters)
            {
                character.SetCharacterRaftPosition();
            }

            raftBuildService.SetEndRaftPoint(spawnPoint.LadderPoint);
            
            OnRaftTransferingEnding?.Invoke();
        }
    }
}
