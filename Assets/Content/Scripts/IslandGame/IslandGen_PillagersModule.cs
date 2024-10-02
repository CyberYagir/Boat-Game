using System;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;
using Random = System.Random;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.IslandGame
{
    public class IslandGen_PillagersModule : IslandGen_SpawnModule
    {
        
        [SerializeField] private IslandGen_VillageModule villageModule;
        [SerializeField] private Range pillagersSpotsCount;
        public override bool SpawnAfterConstruct() => false;
        public override void Init(IslandGenerator islandGenerator)
        {
            base.Init(islandGenerator);
            if (!villageModule.IsSpawned)
            {
                var rnd = new Random(islandGenerator.Seed);
                var count = rnd.Next((int)pillagersSpotsCount.min, (int)pillagersSpotsCount.max);
                for (int i = 0; i < count; i++)
                {
                    var spawned = SpawnObject(prefab) as PillagersSpawner;
                    spawned?.Init(rnd, islandGenerator.TargetIslandData.Level);
                }
            }
        }
    }
}
