using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.QuestsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame
{
    class IslandGen_VillageModule : IslandGeneratorModule
    {
        [SerializeField] private IslandNativesData islandNativesData;
        private PrefabSpawnerFabric spawnerFabricService;

        public bool IsSpawned => islandNativesData.Data.IsSpawned;
        
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
            islandNativesData.Init(islandGenerator.TargetRandom, spawnerFabricService, islandGenerator);
            
            if (islandNativesData.Data.IsSpawned)
            {
                islandGenerator.ClearObjectsInBounds(islandNativesData.Data.Bounds);
                StartCoroutine(SkipFrameAndFindVillage());
            }
        }

        IEnumerator SkipFrameAndFindVillage()
        {
            while (true)
            {
                yield return null;
                QuestsEventBus.CallFindVillage();
            }
        }

        private void OnDrawGizmos()
        {
            islandNativesData.Gizmo();
        }
    }
}