using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Services;
using UnityEngine;

namespace Content.Scripts.IslandGame.Mobs
{
    public class BotSpawnersManager : MonoBehaviour
    {
        private List<BotSpawner> spawners = new List<BotSpawner>();
        public void Init(GameDataObject gameData, PrefabSpawnerFabric prefabSpawner, TerrainBiomeSO biome, IslandMobsService islandMobsService)
        {
            spawners.AddRange(GetComponentsInChildren<BotSpawner>().ToList());
            for (int i = 0; i < spawners.Count; i++)
            {
                spawners[i].Init(gameData, prefabSpawner, biome, islandMobsService);
            }
        }

        public void AddMobSpawner(BotSpawner objMobSpawner)
        {
            if (objMobSpawner != null && objMobSpawner.gameObject.activeInHierarchy)
            {
                spawners.Add(objMobSpawner);
            }
        }
    }
}
