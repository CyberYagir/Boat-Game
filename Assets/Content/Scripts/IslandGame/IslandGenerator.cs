using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        
        private float[,,] alphamaps;
        private float grassY;
        private Dictionary<int, DetailsSO> detailsIds = new Dictionary<int, DetailsSO>(10);
        private void Awake()
        {
            Init(seed);
            // StartCoroutine(Loop());
        }

        public void Init(int seed)
        {
            Random rnd = new Random(seed);

            var terr = SelectTargetTerrain(rnd);
            grassY = terr.transform.InverseTransformPoint(new Vector3(0, minGrassHeight, 0)).y;
            var temperature = rnd.Next((int) temperaturesRange.min, (int) temperaturesRange.max) + terr.TemperatureAdd;
            var targetBiome = SelectBiome(temperature);

            terr.Terrain.terrainData.terrainLayers = targetBiome.Layers;

            PlaceGrass(terr, targetBiome, rnd);

        }
        
        private void PlaceGrass(IslandData terr, TerrainBiomeSO biome, Random rnd)
        {
            var terrainToPopulate = terr.Terrain;
            var terrainData = terrainToPopulate.terrainData;

            if (biome.DetailsData == null) return;
            
            
            int detailWidth = terrainData.detailWidth;
            int detailHeight = terrainData.detailHeight;
            
            AddDetailsAndTreesToTerrain(biome, rnd, detailWidth, terrainData);

            alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            List<TreeInstance> treesInstsances = new List<TreeInstance>();

            
            int[,] details = new int[detailWidth, detailHeight];
            bool isTreesPlaced = false;
            int grassId = 0;
            
            CollectGrassDictionary(biome, grassId);

            for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
            {
                for (int x = 0; x < detailWidth; x++)
                {
                    for (int y = 0; y < detailHeight; y++)
                    {
                        var percentX = x / (float) detailWidth;
                        var percentY = y / (float) detailWidth;


                        var textureLayer = GetTextureLayerID(terrainData, percentX, percentY, biome);
                        var height = GetHeight(terrainData, percentX, percentY);
                        var angle = Vector3.Angle(terrainData.GetInterpolatedNormal(percentX, percentY), Vector3.up);
                        var detailData = detailsIds[i];

                        details[y, x] = 0;

                        if (IsCanPlaceGrass(detailData, height, textureLayer, angle))
                        {
                            details[y, x] = 1024;
                        }



                        if (!isTreesPlaced)
                        {
                            for (int j = 0; j < biome.TreesData.Count; j++)
                            {
                                if (IsCanPlaceGrass(biome.TreesData[j], height, textureLayer, angle))
                                {
                                    if (PlaceTree(biome, j, biome.TreesData[j], rnd, biome.TreesData[j].Noise.GeneratedNoise, x, y, percentX, percentY, treesInstsances, terrainData))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                isTreesPlaced = true;
                terrainData.SetDetailLayer(0, 0, i, details);
            }



            terrainData.SetTreeInstances(treesInstsances.ToArray(), true);
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

        private static void AddDetailsAndTreesToTerrain(TerrainBiomeSO biome, Random rnd, int detailWidth, TerrainData terrainData)
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

        private bool PlaceTree(TerrainBiomeSO biome, int objectIndex, TreesSO treesSO, Random rnd, float[,] noise, int x, int y, float percentX, float percentY, List<TreeInstance> treesInstsances, TerrainData terrainData)
        {

            if (treesSO.Noise.IsInRange(noise[x, y]))
            {
                var pos = new Vector3(percentX, 0.5f, percentY);
                var scale = treesSO.GetRandomScale(rnd);
                var itemID = 0;
                for (int i = 0; i < objectIndex; i++)
                {
                    itemID += biome.TreesData[i].Count;
                }

                itemID += rnd.Next(1, biome.TreesData[objectIndex].Count - 1);

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

                return true;
            }

            return false;
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

        private TerrainLayer GetTextureLayerID(TerrainData terrainData, float percentX, float percentY, TerrainBiomeSO terrainBiome)
        {
            var alphamapX = Mathf.RoundToInt(percentX * (terrainData.alphamapWidth - 1));
            var alphamapY = Mathf.RoundToInt(percentY * (terrainData.alphamapHeight - 1));

            var targetLayerIndex = GetTargetTerrainTexture(alphamapX, alphamapY);
            var textureLayer = terrainBiome.Layers[targetLayerIndex];
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
            for (int i = 0; i < terrains.Length; i++)
            {
                terrains[i].gameObject.SetActive(i == terrainID);
            }

            return terrains[terrainID];
        }


        IEnumerator Loop()
        {
            int i = 0;
            while (true)
            {
                Init(i);
                yield return new WaitForSeconds(0.2f);
                i++;
            }

        }
    }
}
