using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Services;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Map;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    [Serializable]
    public class IslandNativesData : GenerateObjectCalculator
    {
        [Serializable]
        public class VillageData
        {
            [SerializeField] private bool isSpawned;
            [SerializeField] private Bounds bounds;


            public VillageData(bool isSpawned, Bounds bounds)
            {
                this.isSpawned = isSpawned;
                this.bounds = bounds;
            }

            public Bounds Bounds => bounds;

            public bool IsSpawned => isSpawned;
        }
        
        [Serializable]
        public class Villages
        {
            [SerializeField] private TerrainBiomeSO[] biomes;
            [SerializeField] private List<VillageGenerator> structures;
            [SerializeField] private List<TerrainLayer> terrainLayers;

            public List<TerrainLayer> TerrainLayers => terrainLayers;
            public List<VillageGenerator> Structures => structures;

            public TerrainBiomeSO[] Biomes => biomes;
        }

        [SerializeField] private List<Villages> structures = new List<Villages>();

        [SerializeField] private AnimationCurve islandLevelChance;

        private VillageData villageData;
        private IslandData targetTerrain;
        private SaveDataObject saveData;
        private GameDataObject gameDataObject;
        private IslandSeedData islandData;
        private IslandMobsService islandMobsService;
        private VillageGenerator village;

        public VillageData Data => villageData;

        public VillageGenerator Village => village;


        public void Init(
            Random rnd,
            PrefabSpawnerFabric spawner,
            IslandGenerator islandGenerator)
        {
            this.islandGenerator = islandGenerator;
            
            islandMobsService = islandGenerator.IslandMobSpawnService;
            saveData = islandGenerator.SaveData;
            islandData = islandGenerator.TargetIslandData;
            targetTerrain = islandGenerator.TargetTerrain;
            

            var targetChance = islandLevelChance.Evaluate(islandData.Level);

            if (rnd.NextDouble() < targetChance)
            {
                SpawnVillage(islandGenerator.TargetBiome, rnd, islandGenerator.Seed, spawner, islandGenerator.GameData);
            }
            else
            {
                villageData = new VillageData(false, new Bounds());
            }
        }

        private void SpawnVillage(TerrainBiomeSO biome, Random rnd, int seed, PrefabSpawnerFabric spawner, GameDataObject gameData)
        {
            var holder = structures.Find(x => x.Biomes.Contains(biome));
            
            if (holder == null) return;
            
            
            village = Object.Instantiate(holder.Structures.GetRandomItem(rnd));
            village.transform.SetYPosition(0);
            village.Init(biome, rnd, seed, spawner, saveData, gameData, islandData, islandMobsService);
            var startSize = new Vector3(targetTerrain.Terrain.terrainData.size.x, 0, targetTerrain.Terrain.terrainData.size.z);

            villageData = new VillageData(false, new Bounds());

            List<Vector3> points = new List<Vector3>();

            var bounds = targetTerrain.GetBounds();
            for (int lenX = 0; lenX < accuracy; lenX++)
            {
                for (int lenY = 0; lenY < accuracy; lenY++)
                {
                    RandomPos(rnd, village.transform, bounds);

                    if (!bounds.Contains(village.transform.position))
                    {
                        continue;
                    }
                    
                    points.Clear();
                    bool isAllOk = true;
                    foreach (var spawned in village.HousePoints)
                    {
                        if (!CalculateSurface(spawned.transform, holder.TerrainLayers, biome, out Vector3 hitPoint))
                        {
                            isAllOk = false;
                            break;
                        }
                        
                        if (hitPoint != default)
                        {
                            points.Add(hitPoint);
                        }
                    }

                    if (isAllOk)
                    {
                        isAllOk = IsGapAvailable(points);
                    }

                    if (isAllOk)
                    {
                        saveData.GetTargetIsland().AddVillage(village.Uid);
                        village.SpawnHouses();
                        villageData = new VillageData(true, village.GetBounds());
                        return;
                    }


                }
            }

            Object.Destroy(village.gameObject);

        }




        public void Gizmo()
        {
            if (Data != null && Data.IsSpawned)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
                Gizmos.DrawCube(Data.Bounds.center, Data.Bounds.size);
            }
        }
    }
}