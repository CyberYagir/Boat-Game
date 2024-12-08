using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.Services;
using Content.Scripts.Mobs.Mob;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class PillagersSpawner : IslandSpawnedBuild
    {
        [SerializeField] private PrimitiveVillageStructureData structure;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private List<SpawnedMob> pillagers = new List<SpawnedMob>();
        [SerializeField] private GroundedStructure groundStructure;
        private GameDataObject gameDataObject;
        private IslandMobsService islandMobsService;

        [Inject]
        private void Construct(GameDataObject gameDataObject, IslandMobsService islandMobsService)
        {
            this.islandMobsService = islandMobsService;
            this.gameDataObject = gameDataObject;
        }

        public void Init(System.Random rnd, int level)
        {
            groundStructure.Place();
            structure.Init(rnd);
            for (var i = 0; i < structure.NativeSits.Count; i++)
            {
                structure.NativeSits[i].Place();
            }


            var mobs = gameDataObject.NativesListData.GetPillagersCount(rnd, level);

            for (int i = 0; i < mobs.Count; i++)
            {
                islandMobsService.AddMob(rnd, mobs[i], spawnPoints.GetRandomItem().position, LayerMask.GetMask("Terrain"), transform)
                    .With(x => pillagers.Add(x))
                    .With(x => x.Init(null))
                    .With(x=>(x as NativeEnemy).InitPillager(rnd));
            }
        }
    }
}
