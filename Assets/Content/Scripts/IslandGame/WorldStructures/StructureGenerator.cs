using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.PlayerActions;
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
            [System.Serializable]
            public class SubStructure
            {
                [SerializeField] private Structure structure;
                [SerializeField] private float weight;

                public float Weight => weight;
                public Structure Structure => structure;
            }
            [SerializeField] private TerrainBiomeSO biome;
            [SerializeField] private List<SubStructure> structures;

            public List<SubStructure> Structures => structures;

            public TerrainBiomeSO Biome => biome;
        }

        [SerializeField] private RoadsGenerator roadsGenerator;
        [SerializeField] private List<SubStructures> structures = new List<SubStructures>();
        [SerializeField] private List<SubStructures> startStructure;
        private TerrainBiomeSO biome;
        private Random rnd;
        private PrefabSpawnerFabric spawnerFabric;

        public List<RoadBuilder> HousePoints => roadsGenerator.Ends;


        public void Init(TerrainBiomeSO biome, Random rnd, int seed, PrefabSpawnerFabric spawnerFabric)
        {
            this.spawnerFabric = spawnerFabric;
            this.rnd = rnd;
            this.biome = biome;
            roadsGenerator.SpawnRoad(seed);
        }

        public void SpawnHouses()
        {
            foreach (var roadBuilder in HousePoints)
            {
                SpawnStructure(biome, rnd, spawnerFabric, roadBuilder.transform, structures);
            }

            SpawnStructure(biome, rnd, spawnerFabric, transform, startStructure);
        }

        private void SpawnStructure(TerrainBiomeSO biome, Random rnd, PrefabSpawnerFabric spawnerFabric, Transform roadBuilder, List<SubStructures> list)
        {
            List<float> weights = new List<float>();

            var items = list.Find(x => x.Biome == biome);
            for (int i = 0; i < items.Structures.Count; i++)
            {
                weights.Add(items.Structures[i].Weight);
            }
            
            weights.RecalculateWeights();
            var index = weights.ChooseRandomIndexFromWeights(rnd);

            var structure = items.Structures[index];


            var spawnPos = roadBuilder.transform.position;
            if (Physics.Raycast(roadBuilder.transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Terrain"), QueryTriggerInteraction.Ignore))
            {
                spawnPos = hit.point + Vector3.up * 0.2f;
            }

            var spawned = Instantiate(structure.Structure, spawnPos, roadBuilder.transform.rotation, transform);
            spawnerFabric.InjectComponent(spawned.GetComponent<ActionsHolder>());
            spawned.Init(rnd, biome);
        }

        public Bounds GetBounds()
        {
            var b = new Bounds();
            for (int i = 0; i < roadsGenerator.Ends.Count; i++)
            {
                if (b == new Bounds())
                {
                    var pos = roadsGenerator.Ends[i].transform.position;
                    pos.y = 0;
                    b = new Bounds(pos, new Vector3(1, 150, 1));
                }
                else
                {
                    b.Encapsulate(roadsGenerator.Ends[i].transform.position);
                }
            }

            b.Expand(10);

            return b;
        }
    }
}
