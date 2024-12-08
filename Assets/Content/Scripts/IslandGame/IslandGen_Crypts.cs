using System.Collections.Generic;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class IslandGen_Crypts : IslandGen_SpawnModule
    {
        [SerializeField] private List<IslandSpawnedBuild> prefabs = new List<IslandSpawnedBuild>();
        [SerializeField] private TextAsset dungeonNames;
        [SerializeField] private IslandGen_VillageModule villageModule;
        public override void Init(IslandGenerator islandGenerator)
        {
            this.islandGenerator = islandGenerator;
            generateObjectCalculator.SetIslandGenerator(islandGenerator);
            if (SpawnAfterConstruct())
            {
                if (villageModule.IsSpawned)
                {
                    var island = islandGenerator.SaveData.GetTargetIsland();
                    var level = islandGenerator.TargetIslandData.Level;
                    var rnd = new System.Random(island.IslandSeed);
                    foreach (var village in island.VillagesData)
                    {
                        
                        var dungeons =
                            DungeonDataHolder.GenerateDungeons(village, dungeonNames, islandGenerator.GameData, level);

                        for (int i = 0; i < dungeons.Count; i++)
                        {
                            
                            var obj = SpawnObject(prefabs[rnd.Next(0, prefabs.Count)]) as Crypt;
                            if (obj != null)
                            {
                                obj.Init(dungeons[i].Seed, dungeons[i].Level, dungeons[i].Name,
                                    islandGenerator.SaveData);

                                obj.GetComponent<GroundedStructure>().Place();
                                if (!obj.IsCompleted)
                                {
                                    islandGenerator.TargetTerrain.BotsSpawnerManager.AddMobSpawner(obj.MobSpawner);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
