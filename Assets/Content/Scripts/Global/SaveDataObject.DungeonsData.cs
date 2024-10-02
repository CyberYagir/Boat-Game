using System;
using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class DungeonsData
        {
            [Serializable]
            public class DungeonData
            {
                [SerializeField] private int seed;
                [SerializeField] private List<string> destroyedUrns = new List<string>();
                [SerializeField] private List<string> deadMobs = new List<string>();
                [SerializeField] private int allMobsCount;
                [SerializeField] private int allDungeonMobsCount;
                [SerializeField] private bool isBossKilled;


                public DungeonData(int seed)
                {
                    this.seed = seed;
                }

                public int AllMobsCount => allMobsCount;

                public int DeadMobs => deadMobs.Count;

                public int Seed => seed;

                public int AllDungeonMobsCount => allDungeonMobsCount;
                public bool IsBossDead => isBossKilled;


                public void AddUrn(string uid) => destroyedUrns.Add(uid);
                public void AddMob(string uid) => deadMobs.Add(uid);

                public void SetMobsCount(int cout)
                {
                    if (allMobsCount < cout)
                    {
                        allMobsCount = cout;
                    }
                }
                
                public void SetDungeonMobsCount(int cout)
                {
                    if (allDungeonMobsCount < cout)
                    {
                        allDungeonMobsCount = cout;
                    }
                }

                public bool HasUrn(string uid)
                {
                    return destroyedUrns.Contains(uid);
                }

                public bool HasMob(string uid)
                {
                    return deadMobs.Contains(uid);
                }

                public void DefeatBoss()
                {
                    isBossKilled = true;
                }
            }

            [SerializeField] private List<DungeonData> dungeons = new List<DungeonData>();


            public DungeonData RegisterDungeon(int seed)
            {
                var dungeon = dungeons.Find(x => x.Seed == seed);
                if (dungeon == null)
                {
                    dungeon = new DungeonData(seed);
                    dungeons.Add(dungeon);
                }

                return dungeon;
            }

            public DungeonData GetDungeonBySeed(int dataSeed)
            {
                return dungeons.Find(x => x.Seed == dataSeed);
            }
        }
    }
}