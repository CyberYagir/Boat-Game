using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.IslandGame.Mobs
{
    public class BotSpawnersManager : MonoBehaviour
    {
        [SerializeField] private List<BotSpawner> spawners;

        private void OnValidate()
        {
            spawners = GetComponentsInChildren<BotSpawner>().ToList();
        }

        public void Init(GameDataObject gameData, PrefabSpawnerFabric prefabSpawner, TerrainBiomeSO biome)
        {
            for (int i = 0; i < spawners.Count; i++)
            {
                spawners[i].Init(gameData, prefabSpawner, biome);
            }
        }
    }
}
