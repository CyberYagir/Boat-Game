using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    class IsVillageModule : IslandGeneratorModule
    {
        [SerializeField] private IslandNativesData islandNativesData;
        private PrefabSpawnerFabric spawnerFabricService;
        
        [Inject]
        private void Construct(PrefabSpawnerFabric spawnerFabricService)
        {
            this.spawnerFabricService = spawnerFabricService;
        }
        public override void Init(IslandGenerator islandGenerator)
        {
            base.Init(islandGenerator);
            SpawnVillage();
        }

        private void SpawnVillage()
        {
            islandNativesData.Init(
                islandGenerator.Seed,
                islandGenerator.TargetRandom,
                islandGenerator.TargetBiome,
                spawnerFabricService,
                islandGenerator.TargetTerrain,
                islandGenerator,
                islandGenerator.TargetIslandData);


            var targetTerrain = islandGenerator.TargetTerrain;
            var treesInstances = islandGenerator.TreesInstances;
            var spawnedTerrainObjects = islandGenerator.SpawnedTerrainObjects;


            if (islandNativesData.Data.IsSpawned)
            {
                List<int> ids = new List<int>();
                var terrainData = targetTerrain.Terrain.terrainData;
                for (int i = 0; i < treesInstances.Count; i++)
                {
                    var treeInstancePos = treesInstances[i].position;

                    var localPos = new Vector3(treeInstancePos.x * terrainData.size.x, treeInstancePos.y * terrainData.size.y, treeInstancePos.z * terrainData.size.z);
                    var worldPos = Terrain.activeTerrain.transform.TransformPoint(localPos);

                    if (islandNativesData.Data.Bounds.Contains(new Vector3(worldPos.x, 5, worldPos.z)))
                    {
                        ids.Add(i);
                    }
                }


                int n = 0;
                while (ids.Count != 0)
                {
                    treesInstances.RemoveAt(ids[0] - n);
                    if (spawnedTerrainObjects[ids[0] - n] != null)
                    {
                        Destroy(spawnedTerrainObjects[ids[0] - n].gameObject);
                    }

                    spawnedTerrainObjects.RemoveAt(ids[0] - n);

                    ids.RemoveAt(0);
                    n++;
                }

                for (int i = 0; i < spawnedTerrainObjects.Count; i++)
                {
                    if (spawnedTerrainObjects[i] != null)
                    {
                        spawnedTerrainObjects[i].ChangeInstanceID(i);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            islandNativesData.Gizmo();
        }
    }
}