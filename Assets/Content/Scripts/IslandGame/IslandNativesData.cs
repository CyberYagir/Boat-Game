using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.WorldStructures;
using Content.Scripts.Map;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    [System.Serializable]
    public class IslandNativesData
    {
        [System.Serializable]
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
        
        [System.Serializable]
        public class Villages
        {
            [SerializeField] private TerrainBiomeSO[] biomes;
            [SerializeField] private List<StructureGenerator> structures;
            [SerializeField] private List<TerrainLayer> terrainLayers;

            public List<TerrainLayer> TerrainLayers => terrainLayers;
            public List<StructureGenerator> Structures => structures;

            public TerrainBiomeSO[] Biomes => biomes;
        }

        [SerializeField] private List<Villages> structures = new List<Villages>();

        [SerializeField] private AnimationCurve islandLevelChance;

        private VillageData villageData;
        private IslandData targetTerrain;
        private IslandGenerator islandGenerator;
        
        [SerializeField] private int accuracy = 50;
        [SerializeField] private float maxGap = 5;
        [SerializeField] private float maxAngle = 15;
        [SerializeField] private float minY = 1f;
        [SerializeField] private float terrainBorder = 0.1f;

        public VillageData Data => villageData;


        public void Init(
            int seed,
            Random rnd,
            SaveDataObject saveDataObject,
            TerrainBiomeSO biome,
            PrefabSpawnerFabric spawner,
            IslandData targetTerrain,
            IslandGenerator islandGenerator
        )
        {
            this.islandGenerator = islandGenerator;
            this.targetTerrain = targetTerrain;

            var island = saveDataObject.Map.Islands.Find(x => x.IslandSeed == seed);
            var newData = new MapIsland.IslandData(island.IslandPos);

            var targetChance = islandLevelChance.Evaluate(newData.Level);

            if (rnd.NextDouble() < targetChance)
            {
                SpawnVillage(biome, rnd, seed, spawner);
            }
        }

        private void SpawnVillage(TerrainBiomeSO biome, Random rnd, int seed, PrefabSpawnerFabric spawner)
        {
            var holder = structures.Find(x => x.Biomes.Contains(biome));
            
            if (holder == null) return;
            
            
            var village = Object.Instantiate(holder.Structures.GetRandomItem(rnd));
            village.transform.SetYPosition(0);
            village.Init(biome, rnd, seed, spawner);
            var startSize = new Vector3(targetTerrain.Terrain.terrainData.size.x, 0, targetTerrain.Terrain.terrainData.size.z);

            villageData = new VillageData(false, new Bounds());

            List<Vector3> points = new List<Vector3>();

            var bounds = targetTerrain.GetBounds();
            for (int lenX = 0; lenX < accuracy; lenX++)
            {
                for (int lenY = 0; lenY < accuracy; lenY++)
                {
                    RandomPos(rnd, village, startSize);

                    if (!bounds.Contains(village.transform.position))
                    {
                        continue;
                    }
                    
                    points.Clear();
                    bool isAllOk = true;
                    foreach (var spawned in village.HousePoints)
                    {
                        var pos = targetTerrain.Terrain.transform.InverseTransformPoint(spawned.transform.position);

                        var onTerrainPos = pos.DevideVector3(startSize);

                        if (onTerrainPos.x > 1f - terrainBorder || onTerrainPos.x < terrainBorder || onTerrainPos.z > 1f - terrainBorder || onTerrainPos.z < terrainBorder)
                        {
                            isAllOk = false;
                            break;
                        }

                        if (!Physics.Raycast(spawned.transform.position + Vector3.up * 500, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                        {
                            isAllOk = false;
                            DrawDebugIndicator(village, Color.red);
                            break;
                        }

                        if (hit.point.y < minY)
                        {
                            isAllOk = false;
                            DrawDebugIndicator(village, Color.blue);
                            break;
                        }

                        points.Add(hit.point);

                        var layer = islandGenerator.GetTextureLayerID(targetTerrain.Terrain.terrainData, onTerrainPos.x, onTerrainPos.z, biome, out int i);
                        if (!holder.TerrainLayers.Contains(layer))
                        {
                            DrawDebugIndicator(village, Color.magenta);
                            isAllOk = false;
                            break;
                        }

                        if (islandGenerator.GetAngle(
                            (int) (onTerrainPos.x * targetTerrain.Terrain.terrainData.detailResolution),
                            (int) (onTerrainPos.z * targetTerrain.Terrain.terrainData.detailResolution)) > maxAngle)
                        {
                            DrawDebugIndicator(village, Color.yellow);
                            isAllOk = false;
                            break;
                        }
                    }

                    int maxCalGap = 0;
                    for (int i = 0; i < points.Count; i++)
                    {
                        for (int j = 0; j < points.Count; j++)
                        {
                            var delta = Mathf.Abs(points[i].y - points[j].y);
                            if (delta > maxCalGap)
                            {
                                maxCalGap = (int) delta;

                                if (maxCalGap > maxGap)
                                {
                                    DrawDebugIndicator(village, Color.white);
                                    isAllOk = false;
                                    break;
                                }
                            }
                        }

                        if (!isAllOk) break;
                    }
                    
                    if (isAllOk)
                    {
                        village.SpawnHouses();
                        villageData = new VillageData(true, village.GetBounds());
                        return;
                    }


                }
            }

            Object.Destroy(village.gameObject);

        }

        private static void DrawDebugIndicator(StructureGenerator village, Color color)
        {
            var end = village.transform.position + Vector3.down * 500;
            end.y = 0;
            Debug.DrawLine(village.transform.position + Vector3.up * 50, end, color, 5);
        }

        private void RandomPos(Random rnd, StructureGenerator village, Vector3 startSize)
        {
            var percentPos = (new Vector3((float)rnd.NextDouble(), 0, (float)rnd.NextDouble()));
            village.transform.position = targetTerrain.Terrain.transform.TransformPoint(percentPos.MultiplyVector3(startSize));
            
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