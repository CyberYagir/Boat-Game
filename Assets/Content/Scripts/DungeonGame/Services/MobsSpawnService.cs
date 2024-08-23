using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.DungeonGame.Mobs;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame.Services
{
    public class MobsSpawnService : MonoBehaviour
    {
        [SerializeField] private List<DungeonMobSpawner> spawners = new List<DungeonMobSpawner>();
        private System.Random rnd;
        private PrefabSpawnerFabric spawnerFabric;
        private DungeonTileGenerationService tileGenerationService;
        private DungeonService dungeonService;

        [Inject]
        private void Construct(GameDataObject gameData, DungeonService dungeonService, PrefabSpawnerFabric fabric, DungeonTileGenerationService tileGenerationService)
        {
            this.dungeonService = dungeonService;
            this.tileGenerationService = tileGenerationService;
            spawnerFabric = fabric;
            rnd = new System.Random(dungeonService.Seed);
            
        }

        public void AddSpawner(DungeonMobSpawner dungeonMobSpawner)
        {
            if (rnd.NextDouble() <= dungeonMobSpawner.SpawnChance)
            {
                spawners.Add(dungeonMobSpawner);
                if (tileGenerationService.IsGenerated)
                {
                    SpawnMob(dungeonMobSpawner);
                }
                else
                {
                    tileGenerationService.OnLevelGenerated += delegate
                    {
                        SpawnMob(dungeonMobSpawner);
                    };
                }
            }
        }

        private void SpawnMob(DungeonMobSpawner dungeonMobSpawner)
        {
            spawnerFabric.SpawnItem(dungeonService.TargetConfig.GetMob(dungeonMobSpawner.Difficult, rnd).Prefab, dungeonMobSpawner.transform.position, Quaternion.Euler(0, Random.value * 360f, 0), transform);
        }
        

    }
}
