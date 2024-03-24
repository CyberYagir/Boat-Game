using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class StructureGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class SubStructures
        {
            [SerializeField] private TerrainBiomeSO biome;
            [SerializeField] private List<Structure> structures;

            public List<Structure> Structures => structures;

            public TerrainBiomeSO Biome => biome;
        }

        [SerializeField] private RoadsGenerator roadsGenerator;
        [SerializeField] private List<SubStructures> structures = new List<SubStructures>();
        [SerializeField] private List<SubStructures> startStructure;
        public void Init(TerrainBiomeSO biome, Random rnd, int seed, PrefabSpawnerFabric spawnerFabric)
        {
            var ends = roadsGenerator.SpawnRoad(seed);
            foreach (var roadBuilder in ends)
            {
                SpawnStructure(biome, rnd, spawnerFabric, roadBuilder.transform, structures);
            }
            
            SpawnStructure(biome, rnd, spawnerFabric, transform, startStructure);
        }

        private void SpawnStructure(TerrainBiomeSO biome, Random rnd, PrefabSpawnerFabric spawnerFabric, Transform roadBuilder, List<SubStructures> subStructuresList)
        {
            var items = structures.Find(x => x.Biome == biome);
            var structure = items.Structures.GetRandomItem(rnd);

            spawnerFabric.SpawnItemOnGround(structure, roadBuilder.transform.position, roadBuilder.transform.rotation);
        }
    }
}
