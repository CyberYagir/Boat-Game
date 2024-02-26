using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Services;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    public class IslandTransferRaftService : MonoBehaviour
    {
        [SerializeField] private Transform raftsSpawnPoint;
        [SerializeField] private RaftLadderToIsland ladderPrefab;
        [SerializeField] private Transform camera;
        
        private IslandGenerator islandGenerator;
        private RaftBuildService raftBuildService;
        private CharacterService characterService;

        [Inject]
        public void Construct(IslandGenerator islandGenerator, RaftBuildService raftBuildService, CharacterService characterService, CameraMovingService cameraMovingService)
        {
            this.characterService = characterService;
            this.raftBuildService = raftBuildService;
            this.islandGenerator = islandGenerator;
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

            Instantiate(ladderPrefab).With(x => x.Init(spawnPoint.Point.position, spawnPoint.LadderPoint.position));
            
            islandGenerator.BuildNavMesh();
            
            foreach (var ch in characterService.SpawnedCharacters)
            {
                ch.SetCharacterRaftPosition();
            }
        }
    }
}
