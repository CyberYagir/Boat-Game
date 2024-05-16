using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class IslandGen_WaterSource : IslandGeneratorModule
    {
        [SerializeField] private GameObject waterSourcePrefab;
        [SerializeField] private GenerateObjectCalculator generateObjectCalculator;
        [SerializeField] private List<TerrainLayer> allowedLayers = new List<TerrainLayer>();
        private PrefabSpawnerFabric spawnerFabricService;
        private Bounds spawnedBounds;

        [Inject]
        private void Construct(PrefabSpawnerFabric spawnerFabricService)
        {
            this.spawnerFabricService = spawnerFabricService;
        }

        public override void Init(IslandGenerator islandGenerator)
        {
            base.Init(islandGenerator);
            generateObjectCalculator.SetIslandGenerator(islandGenerator);
            SpawnWaterStream();
        }

        private void SpawnWaterStream()
        {
            var spawned = spawnerFabricService.SpawnItem(waterSourcePrefab);
            spawned.gameObject.SetActive(false);
            var bounds = islandGenerator.TargetTerrain.GetBounds();
            for (int lenX = 0; lenX < generateObjectCalculator.Accuracy; lenX++)
            {
                generateObjectCalculator.RandomPos(islandGenerator.TargetRandom, spawned.transform, bounds);

                if (generateObjectCalculator.CalculateSurface(spawned.transform, allowedLayers, islandGenerator.TargetBiome, out Vector3 point))
                {
                    spawned.transform.position = point;
                    break;
                }

                if (lenX == generateObjectCalculator.Accuracy)
                {
                    return;
                }
            }

            spawnedBounds = spawned.GetComponentInChildren<MeshRenderer>().bounds;
            spawnedBounds.Expand(Vector3.up * 100);
            spawnedBounds.Expand(2f);
            islandGenerator.ClearObjectsInBounds(spawnedBounds);
            
            spawned.gameObject.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            Gizmos.DrawCube(spawnedBounds.center, spawnedBounds.size);
        }
    }
}
