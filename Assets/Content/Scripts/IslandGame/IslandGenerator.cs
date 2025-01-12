using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Scriptable;
using Content.Scripts.IslandGame.Services;
using Content.Scripts.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.IslandGame
{
    public class IslandGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;
        [SerializeField] private List<IslandGeneratorModule> spawnModules;
        [SerializeField] private float minGrassHeight;
        [SerializeField, ReadOnly] private IslandSeedData islandData;

        [SerializeField] private IslandGenerationDataObject islandGeneration;

        [SerializeField] private TerrainObject terrainObjectIndicator;

        [SerializeField] private TerrainBiomeSO debugBiome;

        private float[,,] alphamaps;
        private int[,] details;
        private float[,] heights;
        private float[,] angles;
        private int[,] textureLayers;
        private int[,] spawnedObjects;

        private Dictionary<int, DetailsSO> detailsIds = new Dictionary<int, DetailsSO>(10);
        private List<TerrainObject> spawnedTerrainObjects = new List<TerrainObject>(1024);
        private List<TreeInstance> treesInstances = new List<TreeInstance>(1000);
        private IslandData targetTerrain;
        private bool isNavMeshBuilded = false;
        private float grassY;
        private Random random;
        private TerrainBiomeSO targetBiome;


        private IslandData currentIslandData;
        private SelectionService selectionService;
        private GameDataObject gameDataObject;
        private SaveDataObject saveDataObject;
        private PrefabSpawnerFabric prefabSpawnerFabric;
        private INavMeshProvider navMeshProvider;
        private TickService tickService;
        private List<Bounds> structuresBounds = new List<Bounds>();


        private Range TemperaturesRange => islandGeneration.IslandTemperatureRange;
        private IslandData[] Terrains => islandGeneration.Terrains;
        private TerrainBiomeSO[] BiomesDatas => islandGeneration.BiomesData;
        
        

        public IslandData CurrentIslandData => currentIslandData;

        public int Seed => seed;

        public Random TargetRandom => random;

        public TerrainBiomeSO TargetBiome => targetBiome;

        public IslandSeedData TargetIslandData => islandData;

        public IslandData TargetTerrain => targetTerrain;

        public List<TreeInstance> TreesInstances => treesInstances;

        public List<TerrainObject> SpawnedTerrainObjects => spawnedTerrainObjects;
        public SaveDataObject SaveData => saveDataObject;
        public GameDataObject GameData => gameDataObject;
        public IslandMobsService IslandMobSpawnService => islandMobsService;

        public List<Bounds> StructuresBounds => structuresBounds;


        [Inject]
        private void Construct(
            SaveDataObject saveDataObject,
            SelectionService selectionService,
            GameDataObject gameDataObject,
            PrefabSpawnerFabric prefabSpawnerFabric,
            IRaftBuildService raftBuildService,
            INavMeshProvider navMeshProvider,
            TickService tickService,
            IslandMobsService islandMobsService,
            StructuresService structuresService)
        {
            this.islandMobsService = islandMobsService;
            this.tickService = tickService;
            this.navMeshProvider = navMeshProvider;
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.gameDataObject = gameDataObject;
            this.selectionService = selectionService;
            this.saveDataObject = saveDataObject;

            seed = saveDataObject.Global.IslandSeed;
            structuresService.SetIsland(this);
            Init(saveDataObject.Global.IslandSeed);
            
            
            raftBuildService.OnChangeRaft += BuildNavMesh;
            if (raftBuildService.SpawnedRafts.Count != 0)
            {
                BuildNavMesh();
            }
        }

        public void BuildNavMesh()
        {
            if (!isNavMeshBuilded)
            {
                navMeshProvider.BuildNavMesh();
                isNavMeshBuilded = true;
            }
            else
            {
                navMeshProvider.BuildNavMeshAsync();
            }
            print("Build Nav Mesh");
        }



        public void Init(int seed)
        {
            var island = saveDataObject.Map.Islands.Find(x => x.IslandSeed == seed);
            islandData = IslandSeedData.GenerateWithAdditionalData(island.IslandPos, Terrains, BiomesDatas, TemperaturesRange);
            
        
            random = new Random(seed);

            targetTerrain = SelectTargetTerrain(islandData.TerrainIndex);
            grassY = TargetTerrain.transform.InverseTransformPoint(new Vector3(0, minGrassHeight, 0)).y;
            targetBiome = islandData.Biome;

#if UNITY_EDITOR

            if (debugBiome != null)
            {
                targetBiome = debugBiome;
            }
#endif

            
            TargetTerrain.Terrain.terrainData.terrainLayers = TargetBiome.Layers;
            
            PlaceAll(TargetTerrain, TargetBiome, TargetRandom, TargetIslandData.Level);


            for (int i = 0; i < spawnModules.Count; i++)
            {
                prefabSpawnerFabric.InjectComponent(spawnModules[i]);
                spawnModules[i].Init(this);
            }

            // SpawnVillage(seed, TargetRandom, TargetBiome, TargetIslandData);
            TargetTerrain.Terrain.terrainData.SetTreeInstances(TreesInstances.ToArray(), true);
            currentIslandData = TargetTerrain;

            currentIslandData.Init(gameDataObject, prefabSpawnerFabric, TargetBiome, islandMobsService);
        }

        private void PlaceAll(IslandData terr, TerrainBiomeSO biome, Random rnd, int islandLevel)
        {
            var terrainToPopulate = terr.Terrain;
            var terrainData = terrainToPopulate.terrainData;

            if (biome.DetailsData == null) return;

            terrainData.SetDetailResolution(512, 64);

            float detailWidth = terrainData.detailWidth;
            float detailHeight = terrainData.detailHeight;

            AddDetailsAndTreesToTerrain(biome, rnd, (int) detailWidth, terrainData);

            alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

            details = new int[(int) detailWidth, (int) detailHeight];
            heights = new float[(int) detailWidth, (int) detailHeight];
            angles = new float[(int) detailWidth, (int) detailHeight];
            textureLayers = new int[(int) detailWidth, (int) detailHeight];
            spawnedObjects = new int[(int) detailWidth, (int) detailHeight];


            bool isFirstLoopEnded = false;
            int grassId = 0;
            var islandData = saveDataObject.Map.GetIslandData(Seed);
            CollectGrassDictionary(biome, grassId);


            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                for (int x = 0; x < detailWidth; x++)
                {
                    for (int y = 0; y < detailHeight; y++)
                    {
                        var percentX = x / detailWidth;
                        var percentY = y / detailWidth;


                        var detailData = detailsIds[i];
                        TerrainLayer textureLayer = null;
                        float height = 0;
                        float angle = 0;

                        details[y, x] = 0;

                        GetOrCacheData(biome, isFirstLoopEnded, terrainData, percentX, percentY, y, x, out height, out angle, out textureLayer);
                        PlaceGrass(detailData, height, textureLayer, angle, y, x);
                        PlaceTrees(terr, biome, rnd, isFirstLoopEnded, y, x, height, textureLayer, angle, percentX, percentY, islandData, islandLevel);

                    }
                }

                isFirstLoopEnded = true;
                terrainData.SetDetailLayer(0, 0, i, details);
            }




        }

        private void PlaceTrees(IslandData terr, TerrainBiomeSO biome, Random rnd, bool isFirstLoopEnded, int y, int x, float height, TerrainLayer textureLayer, float angle, float percentX, float percentY, SaveDataObject.MapData.IslandData islandData, int islandLevel)
        {
            if (!isFirstLoopEnded)
            {
                spawnedObjects[y, x] = -1;
                for (int j = 0; j < biome.TreesData.Count; j++)
                {
                    if (islandLevel >= biome.TreesData[j].MinLevel)
                    {
                        if (IsCanPlaceGrass(biome.TreesData[j], height, textureLayer, angle))
                        {
                            if (PlaceTree(terr.Terrain, biome, j, biome.TreesData[j], rnd, biome.TreesData[j].Noise.GeneratedNoise, x, y, percentX, percentY, TreesInstances, height, islandData))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void PlaceGrass(DetailsSO detailData, float height, TerrainLayer textureLayer, float angle, int y, int x)
        {
            if (IsCanPlaceGrass(detailData, height, textureLayer, angle))
            {
                details[y, x] = detailData.DensityScale;
            }
        }

        private void GetOrCacheData(TerrainBiomeSO biome, bool isFirstLoopEnded, TerrainData terrainData, float percentX, float percentY, int y, int x, out float height, out float angle, out TerrainLayer textureLayer)
        {
            if (!isFirstLoopEnded)
            {
                textureLayer = GetTextureLayerID(terrainData, percentX, percentY, biome, out int index);
                height = GetHeight(terrainData, percentX, percentY);
                angle = Vector3.Angle(terrainData.GetInterpolatedNormal(percentX, percentY), Vector3.up);

                textureLayers[y, x] = index;
                heights[y, x] = height;
                angles[y, x] = angle;
            }
            else
            {
                textureLayer = biome.Layers[textureLayers[y, x]];
                height = heights[y, x];
                angle = angles[y, x];
            }
        }

        private void CollectGrassDictionary(TerrainBiomeSO biome, int grassId)
        {
            for (int d = 0; d < biome.DetailsData.Count; d++)
            {
                for (int n = 0; n < biome.DetailsData[d].Count; n++)
                {
                    detailsIds.Add(grassId, biome.DetailsData[d]);
                    grassId++;
                }
            }
        }

        private void AddDetailsAndTreesToTerrain(TerrainBiomeSO biome, Random rnd, int detailWidth, TerrainData terrainData)
        {
            List<TreePrototype> treePrototypes = new List<TreePrototype>();
            for (int i = 0; i < biome.TreesData.Count; i++)
            {
                treePrototypes.AddRange(biome.TreesData[i].GetTreePrototypes());
                biome.TreesData[i].Noise.GetNoise(rnd.Next(-1000, 1000), detailWidth, detailWidth);
            }

            terrainData.treePrototypes = treePrototypes.ToArray();


            List<DetailPrototype> detailPrototypes = new List<DetailPrototype>();
            for (int i = 0; i < biome.DetailsData.Count; i++)
            {
                detailPrototypes.AddRange(biome.DetailsData[i].GetDetailPrototypes(rnd));
            }

            terrainData.detailPrototypes = detailPrototypes.ToArray();

            terrainData.RefreshPrototypes();
        }

        private bool PlaceTree(Terrain terrain, TerrainBiomeSO biome, int objectIndex, TreesSO treesSO, Random rnd, float[,] noise, int x, int y, float percentX, float percentY, List<TreeInstance> treesInstsances, float height, SaveDataObject.MapData.IslandData islandData)
        {
            if (!treesSO.Noise.IsInRange(noise[x, y]) || !treesSO.IsDensityOk(rnd)) return false;
            var intPos = new Vector2Int(x, y);


            var pos = new Vector3(percentX, 0.5f, percentY);
            var scale = treesSO.GetRandomScale(rnd);
            var itemID = 0;
            for (int i = 0; i < objectIndex; i++)
            {
                itemID += biome.TreesData[i].Count;
            }



            treesSO.PrepareWeights();
            var targetBiomeItem = treesSO.GetRandomTreeIndexByWeight(rnd);
            
            itemID += targetBiomeItem;

            var tree = new TreeInstance()
            {
                color = Color.white,
                position = pos,
                prototypeIndex = itemID,
                heightScale = scale,
                widthScale = scale,
                rotation = rnd.Next(0, 360),
                lightmapColor = Color.white
            };

            if (!islandData.IsTreeDestroyed(intPos))
            {
                treesInstsances.Add(tree);

                var treeData = treesSO.GetObjectByID(targetBiomeItem).GetComponent<TreeData>();
                if (treeData != null)
                {
                    Instantiate(terrainObjectIndicator, terrain.transform.position + new Vector3(percentX * terrain.terrainData.size.x, height, percentY * terrain.terrainData.size.z), Quaternion.identity)
                        .With(z => z.transform.localScale *= scale)
                        .With(z => SpawnedTerrainObjects.Add(z))
                        .With(z => z.Init(
                            treesInstsances.Count - 1,
                            treeData,
                            selectionService,
                            gameDataObject,
                            prefabSpawnerFabric,
                            this,
                            intPos
                        ))
                        .With(z => z.transform.parent = transform);
                }
                else
                {
                    SpawnedTerrainObjects.Add(null);
                }
            }

            spawnedObjects[y, x] = itemID;

            return true;
        }

        private bool IsCanPlaceGrass(ObjectsSO placedItems, float height, TerrainLayer textureLayer, float maxAngle)
        {
            return height > grassY && maxAngle < placedItems.MaxAngle && placedItems.IsCanPlace(textureLayer);
        }

        private float GetHeight(TerrainData terrainData, float percentX, float percentY)
        {
            var heightX = Mathf.RoundToInt(percentX * terrainData.heightmapResolution);
            var heightY = Mathf.RoundToInt(percentY * terrainData.heightmapResolution);

            var height = terrainData.GetHeight(heightX, heightY);
            return height;
        }

        public TerrainLayer GetTextureLayerID(TerrainData terrainData, float percentX, float percentY, TerrainBiomeSO terrainBiome, out int index)
        {
            var alphamapX = Mathf.RoundToInt(percentX * (terrainData.alphamapWidth - 1));
            var alphamapY = Mathf.RoundToInt(percentY * (terrainData.alphamapHeight - 1));

            var targetLayerIndex = GetTargetTerrainTexture(alphamapX, alphamapY);
            var textureLayer = terrainBiome.Layers[targetLayerIndex];
            index = targetLayerIndex;
            return textureLayer;
        }

        private int GetTargetTerrainTexture(int x, int y)
        {
            for (int i = alphamaps.GetLength(2) - 1; i >= 0; i--)
            {
                if (alphamaps[y, x, i] >= 0.7f)
                {
                    return i;
                }
            }

            return 0;
        }

        private TerrainBiomeSO SelectBiome(float temperature)
        {
            for (int i = 0; i < BiomesDatas.Length; i++)
            {
                if (BiomesDatas[i].isInrRange(temperature))
                {
                    return BiomesDatas[i];
                }
            }

            return BiomesDatas.Last();
        }

        private IslandData SelectTargetTerrain(int terrainID)
        {
            return Instantiate(Terrains[terrainID], transform)
                .With(x => x.GetComponent<ActionsHolder>()
                    .Construct(selectionService, gameDataObject));
        }

        public TreeInstance RemoveTree(int id, out float size)
        {
            var instance = TargetTerrain.Terrain.terrainData.GetTreeInstance(id);

            size = instance.heightScale;

            instance.widthScale = 0;
            instance.heightScale = 0;
            TargetTerrain.Terrain.terrainData.SetTreeInstance(id, instance);

            return instance;
        }

        public void RemoveTreeToSave(Vector2Int pos)
        {
            var island = saveDataObject.Map.GetIslandData(Seed);
            island.AddDestroyedTreePos(pos);
        }

        public float GetAngle(int x, int y)
        {
            return angles[y, x];
        }


        List<int> ids = new List<int>(100);
        private IslandMobsService islandMobsService;

        public void ClearObjectsInBounds(Bounds bounds)
        {
            ids.Clear();
            var terrainData = targetTerrain.Terrain.terrainData;
            for (int i = 0; i < treesInstances.Count; i++)
            {
                var treeInstancePos = treesInstances[i].position;

                var localPos = new Vector3(treeInstancePos.x * terrainData.size.x, treeInstancePos.y * terrainData.size.y, treeInstancePos.z * terrainData.size.z);
                var worldPos = targetTerrain.transform.TransformPoint(localPos);

                if (bounds.Contains(new Vector3(worldPos.x, 5, worldPos.z)))
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

        public void AddStructureBounds(Bounds bounds)
        {
            structuresBounds.Add(bounds);
            print("Add structure bounds");
        }

        public bool PointInsideOfStructureBounds(Transform spawnedPoint)
        {
            for (int i = 0; i < structuresBounds.Count; i++)
            {
                if (structuresBounds[i].Contains(spawnedPoint.position))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            for (int i = 0; i < structuresBounds.Count; i++)
            {
                Gizmos.DrawCube(structuresBounds[i].center, structuresBounds[i].size);
            }
        }
    }
}
