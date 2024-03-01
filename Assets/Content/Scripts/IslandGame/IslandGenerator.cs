using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Zenject;
using Random = System.Random;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.IslandGame
{
    public class IslandGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;
        [SerializeField] private IslandData[] terrains;
        [SerializeField] private TerrainBiomeSO[] biomesDatas;
        [SerializeField] private Range temperaturesRange;
        [SerializeField] private float minGrassHeight;


        [SerializeField] private TerrainObject terrainObjectIndicator;
        [SerializeField] private NavMeshSurface navMeshSurface;
        private SaveDataObject saveDataObject;
        
        
        private float[,,] alphamaps;
        private int[,] details;
        private float[,] heights;
        private float[,] angles;
        private int[,] textureLayers;
        private int[,] spawnedObjects;

        private float grassY;
        private Dictionary<int, DetailsSO> detailsIds = new Dictionary<int, DetailsSO>(10);
        private List<TerrainObject> spawnedTerrainObjects = new List<TerrainObject>(1024);
        private bool isNavMeshBuilded = false;


        private IslandData currentIslandData;
        private SelectionService selectionService;
        private GameDataObject gameDataObject;

        public IslandData CurrentIslandData => currentIslandData;

        public int Seed => seed;

        private List<TreeInstance> treesInstsances = new List<TreeInstance>(1000);
        private IslandData targetTerrain;

        [Inject]
        private void Construct(SaveDataObject saveDataObject, SelectionService selectionService, GameDataObject gameDataObject)
        {
            this.gameDataObject = gameDataObject;
            this.selectionService = selectionService;
            this.saveDataObject = saveDataObject;
            
            Init(Seed);
            
            print("execute " + transform.name);
        }

        public void BuildNavMesh()
        {
            if (!isNavMeshBuilded)
            {
                navMeshSurface.BuildNavMesh();
                isNavMeshBuilded = true;
            }
            else
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            }
        }


        public void Init(int seed)
        {
            Random rnd = new Random(seed);

            targetTerrain = SelectTargetTerrain(rnd);
            grassY = targetTerrain.transform.InverseTransformPoint(new Vector3(0, minGrassHeight, 0)).y;
            var temperature = rnd.Next((int) temperaturesRange.min, (int) temperaturesRange.max) + targetTerrain.TemperatureAdd;
            var targetBiome = SelectBiome(temperature);

            print(targetBiome.name + "/t:" + temperature);

            targetTerrain.Terrain.terrainData.terrainLayers = targetBiome.Layers;

            PlaceGrass(targetTerrain, targetBiome, rnd);


            currentIslandData = targetTerrain;
        }

        private void PlaceGrass(IslandData terr, TerrainBiomeSO biome, Random rnd)
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


                        GetOrCacheData(biome, isFirstLoopEnded, terrainData, percentX, percentY, y, x, out height, out angle, out textureLayer);

                        details[y, x] = 0;

                        if (IsCanPlaceGrass(detailData, height, textureLayer, angle))
                        {
                            details[y, x] = detailData.DensityScale;
                        }



                        if (!isFirstLoopEnded)
                        {
                            spawnedObjects[y, x] = -1;
                            for (int j = 0; j < biome.TreesData.Count; j++)
                            {
                                if (IsCanPlaceGrass(biome.TreesData[j], height, textureLayer, angle))
                                {
                                    if (PlaceTree(terr.Terrain, biome, j, biome.TreesData[j], rnd, biome.TreesData[j].Noise.GeneratedNoise, x, y, percentX, percentY, treesInstsances, height))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                isFirstLoopEnded = true;
                terrainData.SetDetailLayer(0, 0, i, details);
            }



            terrainData.SetTreeInstances(treesInstsances.ToArray(), true);

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

        private bool PlaceTree(Terrain terrain, TerrainBiomeSO biome, int objectIndex, TreesSO treesSO, Random rnd, float[,] noise, int x, int y, float percentX, float percentY, List<TreeInstance> treesInstsances, float height)
        {
            if (!treesSO.Noise.IsInRange(noise[x, y]) || !treesSO.IsDensityOk(rnd)) return false;



            var pos = new Vector3(percentX, 0.5f, percentY);
            var scale = treesSO.GetRandomScale(rnd);
            var itemID = 0;
            for (int i = 0; i < objectIndex; i++)
            {
                itemID += biome.TreesData[i].Count;
            }

            var targetBiomeItem = rnd.Next(1, treesSO.Count - 1);
            itemID += targetBiomeItem;

            treesInstsances.Add(new TreeInstance()
            {
                color = Color.white,
                position = pos,
                prototypeIndex = itemID,
                heightScale = scale,
                widthScale = scale,
                rotation = rnd.Next(0, 360),
                lightmapColor = Color.white
            });

            Instantiate(terrainObjectIndicator, terrain.transform.position + new Vector3(percentX * terrain.terrainData.size.x, height, percentY * terrain.terrainData.size.z), Quaternion.identity)
                .With(z => z.transform.localScale *= scale)
                .With(z => spawnedTerrainObjects.Add(z))
                .With(z => z.Init(
                    treesInstsances.Count - 1, 
                    treesSO.GetObjectByID(targetBiomeItem).GetComponent<TreeData>(), 
                    selectionService, 
                    gameDataObject, 
                    this))
                .With(z => z.transform.parent = transform);

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

        private TerrainLayer GetTextureLayerID(TerrainData terrainData, float percentX, float percentY, TerrainBiomeSO terrainBiome, out int index)
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
            for (int i = 0; i < biomesDatas.Length; i++)
            {
                if (biomesDatas[i].isInrRange(temperature))
                {
                    return biomesDatas[i];
                }
            }

            return biomesDatas.Last();
        }

        private IslandData SelectTargetTerrain(Random rnd)
        {
            var terrainID = rnd.Next(0, terrains.Length);
            return Instantiate(terrains[terrainID], transform)
                .With(x => x.GetComponent<ActionsHolder>().Construct(selectionService, gameDataObject));
        }

        public TreeInstance RemoveTree(int id, out float size)
        {
            var instance = targetTerrain.Terrain.terrainData.GetTreeInstance(id);

            size = instance.heightScale;
            
            instance.widthScale = 0;
            instance.heightScale = 0;
            targetTerrain.Terrain.terrainData.SetTreeInstance(id, instance);

            return instance;
        }
        
        
        IEnumerator GoForAllSaves()
        {
            for (int i = 0; i < saveDataObject.Map.Islands.Count; i++)
            {
                detailsIds.Clear();
                spawnedTerrainObjects.Clear();
                print("#" + i + ": " + saveDataObject.Map.Islands[i].IslandSeed);
                Init(saveDataObject.Map.Islands[i].IslandSeed);
                yield return null;
            }
        }
    }
}
