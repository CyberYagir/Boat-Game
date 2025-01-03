using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.IslandGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class VillagePopulation : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<int> allSeeds = new List<int>(20);
        private Dictionary<StructureDataBase, List<int>> unitsSeeds = new Dictionary<StructureDataBase, List<int>>(20);
        private List<int> tmpSeeds = new List<int>(10);
        private List<NativeController> spawnedNatives = new List<NativeController>();

        public List<NativeController> SpawnedNatives => spawnedNatives;

        public void Init(
            List<StructureDataBase> structuresData, 
            System.Random rnd, 
            SaveDataObject saveDataObject, 
            NativesListSO nativesList, 
            PrefabSpawnerFabric prefabSpawnerFabric, 
            string villageID,
            Bounds bounds,
            IslandMobsService islandMobsService)
        {
            GenerateVillagersIds(structuresData, rnd);
            GenerateVillagesBySeeds(saveDataObject, nativesList, prefabSpawnerFabric, villageID, bounds, islandMobsService);
        }

        private void GenerateVillagesBySeeds(SaveDataObject saveDataObject, NativesListSO nativesList, PrefabSpawnerFabric spawnerFabric, string villageID, Bounds bounds, IslandMobsService islandMobsService)
        {
            var island = saveDataObject.GetTargetIsland();
            var village = island.GetVillage(villageID);
            foreach (var item in unitsSeeds)
            {
                if (item.Key.Habitable)
                {
                    var types = item.Key.GetTypes(item.Value.Count);
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var charRnd = new System.Random(item.Value[i]);
                        var uid = Extensions.GenerateSeededGuid(charRnd).ToString();
                        var villager = village.GetVillager(uid);
                        if (villager == null)
                        {
                            village.AddVillager(uid);
                            SpawnCharacter(nativesList, spawnerFabric, charRnd, types[i], item.Key, islandMobsService);
                        }
                        else if (!villager.IsDead)
                        {
                            SpawnCharacter(nativesList, spawnerFabric, charRnd, types[i], item.Key, islandMobsService);
                        }
                    }
                }
            }
        }

        private void SpawnCharacter(NativesListSO nativesList, PrefabSpawnerFabric spawnerFabric, Random charRnd, ENativeType type, StructureDataBase structure, IslandMobsService islandMobsService)
        {
            var prefab = nativesList.GetByType(type, charRnd);

            var enemy = islandMobsService.AddMob(prefab, structure.transform.position, LayerMask.GetMask("Terrain"), structure.transform);
            enemy.Init(null);
            SpawnedNatives.Add(enemy);
        }
        
        private void GenerateVillagersIds(List<StructureDataBase> structuresData, Random rnd)
        {
            for (int i = 0; i < structuresData.Count; i++)
            {
                structuresData[i].Init(rnd);
            }

            for (int i = 0; i < structuresData.Count; i++)
            {
                if (structuresData[i].Habitable)
                {
                    var count = rnd.Next(1, structuresData[i].MaxPeoples);
                    for (int k = 0; k < count; k++)
                    {
                        var uid = 0;
                        do
                        {
                            uid = rnd.Next(-10000, 10000);
                        } while (allSeeds.Contains(uid));

                        allSeeds.Add(uid);
                        tmpSeeds.Add(uid);
                    }

                    unitsSeeds.Add(structuresData[i], new List<int>(tmpSeeds));
                    tmpSeeds.Clear();
                }
            }
        }


        private void Update()
        {
            for (int i = 0; i < SpawnedNatives.Count; i++)
            {
                SpawnedNatives[i].OnUpdate();
            }
        }
    }
}
