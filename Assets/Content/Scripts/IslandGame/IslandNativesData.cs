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
        [SerializeField] private StructureGenerator[] structures;
        [SerializeField] private AnimationCurve islandLevelChance;
        
        

        public void Init(int seed, Random rnd, SaveDataObject saveDataObject, TerrainBiomeSO biome, PrefabSpawnerFabric spawner)
        {
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
            var village = Object.Instantiate(structures.GetRandomItem());
            village.Init(biome, rnd, seed, spawner);
        }
    }
}