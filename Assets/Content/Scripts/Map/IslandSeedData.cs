using System.Collections.Generic;
using System.Linq;
using Content.Scripts.IslandGame;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.Map
{
    [System.Serializable]
    public class IslandSeedData
    {
        [SerializeField, ReadOnly] private int level;
        [SerializeField, ReadOnly] private int terrainIndex;
        [SerializeField, ReadOnly] private int temperature;
        [SerializeField, ReadOnly] private TerrainBiomeSO biome;

        private Random rnd;
        public TerrainBiomeSO Biome => biome;
        public int Temperature => temperature;
        public int TerrainIndex => terrainIndex;

        public int Level => level;

        public IslandSeedData(Vector2Int pos)
        {
            GenerateDefaultData(pos);
        }


        private IslandSeedData(
            Vector2Int pos,
            IslandData[] terrains,
            TerrainBiomeSO[] biomes,
            Range temperatureRange)
        {
            GenerateDefaultData(pos);
            GenerateTerrainIndex(terrains);
            GenerateTemperature(temperatureRange, terrains[terrainIndex]);
            GenerateBiome(biomes);
        }

        private void GenerateBiome(TerrainBiomeSO[] biomes)
        {
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].isInrRange(temperature))
                {
                    biome = biomes[i];
                    return;
                }
            }

            biome = biomes.Last();
        }

        private void GenerateTerrainIndex(IslandData[] terrains)
        {
            terrainIndex = rnd.Next(0, terrains.Length);
        }


        private void GenerateDefaultData(Vector2Int pos)
        {
            rnd = new Random(Mathf.FloorToInt(Mathf.Pow((float) pos.x + pos.y, 2) / 125f));
            var seed = rnd.Next(-100000, 100000);
            rnd = new Random(seed);
            level = rnd.Next(1, 10);
        }

        public void GenerateTemperature(Range range, IslandData terrain)
        {
            temperature = rnd.Next((int) range.min, (int) range.max) + terrain.TemperatureAdd;
        }

        public static IslandSeedData Generate(Vector2Int pos)
        {
            return new IslandSeedData(pos);
        }


        public static IslandSeedData GenerateWithAdditionalData(
            Vector2Int pos,
            IslandData[] terrains,
            TerrainBiomeSO[] biomes,
            Range temperatureRange)
        {
            return new IslandSeedData(pos, terrains, biomes, temperatureRange);
        }

    }
}