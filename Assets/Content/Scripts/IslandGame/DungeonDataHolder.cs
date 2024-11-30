using System.Collections.Generic;
using Content.Scripts.DungeonGame.Services;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    [System.Serializable]
    public class DungeonDataHolder
    {
        [SerializeField] private int seed;
        [SerializeField] private string name;
        [SerializeField] private int level;

        public DungeonDataHolder(int seed, string name, int level)
        {
            this.seed = seed;
            this.name = name;
            this.level = level;
        }

        public int Level => level;

        public string Name => name;

        public int Seed => seed;



        public static List<DungeonDataHolder> GenerateDungeons(
            SaveDataObject.MapData.IslandData.VillageData villageData,
            TextAsset dungeonNames,
            GameDataObject gameDataObject,
            int level
        )
        {
            var dungeons = new List<DungeonDataHolder>();

            var names = dungeonNames.LinesToList();
            var rnd = villageData.GetRandom();

            var dungeonsCount = gameDataObject.ConfigData.GetDungeonsCount(rnd);


            for (int i = 0; i < dungeonsCount; i++)
            {
                var dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                while (dungeon.Level < level - gameDataObject.ConfigData.DungeonsLevelsOffset ||
                       dungeon.Level > level + gameDataObject.ConfigData.DungeonsLevelsOffset || dungeon.Seed == 0)
                {
                    dungeon = new DungeonData(rnd.Next(int.MinValue, int.MaxValue));
                }

                dungeons.Add(new DungeonDataHolder(dungeon.Seed, names.GetRandomItem(rnd), dungeon.Level));
            }

            return dungeons;
        }
    }
}