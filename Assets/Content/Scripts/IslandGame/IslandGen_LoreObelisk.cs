using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    public class IslandGen_LoreObelisk : IslandGeneratorModule
    {
        [SerializeField] private LoreObelisk obiliskPrefab;
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
            if (islandGenerator.SaveData.Map.IsHavePlotOnIsland(islandGenerator.SaveData.GetTargetIsland().IslandSeed))
            {
                generateObjectCalculator.SetIslandGenerator(islandGenerator);
                SpawnObelisk();
            }
        }

        private void SpawnObelisk()
        {
            var spawned = spawnerFabricService.SpawnItem(obiliskPrefab);
            spawned.gameObject.SetActive(false);
            var bounds = islandGenerator.TargetTerrain.GetBounds();
            var points = new List<Vector3>(5);
            for (int lenX = 0; lenX < generateObjectCalculator.Accuracy; lenX++)
            {
                generateObjectCalculator.RandomPos(islandGenerator.TargetRandom, spawned.transform, bounds);

                bool isAllOk = true;

                points.Clear();
                for (int i = 0; i < spawned.Points.Count; i++)
                {
                    if (!generateObjectCalculator.CalculateSurface(spawned.Points[i], allowedLayers, islandGenerator.TargetBiome, out Vector3 point))
                    {
                        isAllOk = false;
                        break;
                    }

                    if (point != default)
                    {
                        points.Add(point);
                    }
                }

                if (isAllOk)
                {
                    isAllOk = generateObjectCalculator.IsGapAvailable(points);
                    var maxY = points.Max(x => x.y);

                    if (isAllOk)
                    {
                        spawned.transform.SetYPosition(maxY);
                        print("Gap Available");
                        break;
                    }
                }
            }
            
            
            
            spawnedBounds = spawned.GetComponentInChildren<MeshRenderer>().bounds;
            spawnedBounds.Expand(Vector3.up * 100);
            spawnedBounds.Expand(2f);
            islandGenerator.ClearObjectsInBounds(spawnedBounds);
            spawned.transform.SetYEulerAngles(islandGenerator.TargetRandom.Next(0, 360));
            spawned.gameObject.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            Gizmos.DrawCube(spawnedBounds.center, spawnedBounds.size);
        }
    }
}
