using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    class IslandGen_VillageModule : IslandGeneratorModule
    {
        [SerializeField] private IslandNativesData islandNativesData;
        private PrefabSpawnerFabric spawnerFabricService;
        
        [Inject]
        private void Construct(PrefabSpawnerFabric spawnerFabricService)
        {
            this.spawnerFabricService = spawnerFabricService;
        }
        public override void Init(IslandGenerator islandGenerator)
        {
            base.Init(islandGenerator);
            SpawnVillage();
        }

        private void SpawnVillage()
        {
            islandNativesData.Init(
                islandGenerator.Seed,
                islandGenerator.TargetRandom,
                islandGenerator.TargetBiome,
                spawnerFabricService,
                islandGenerator.TargetTerrain,
                islandGenerator,
                islandGenerator.TargetIslandData);

            if (islandNativesData.Data.IsSpawned)
            {
                islandGenerator.ClearObjectsInBounds(islandNativesData.Data.Bounds);
            }
        }

        private void OnDrawGizmos()
        {
            islandNativesData.Gizmo();
        }
    }
}