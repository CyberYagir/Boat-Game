using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.Mobs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class Crypt : IslandSpawnedBuild
    {
        [SerializeField] private GameObject cryptNotCompletedVisualsPrefab;
        [SerializeField] private GameObject cryptCompletedVisualsPrefab;
        [SerializeField] private BotSpawner botSpawner;
        [SerializeField] private GameObject[] guards;
        [SerializeField] private List<MobObject.EMobType> mobTypes = new List<MobObject.EMobType>();
        private int seed;
        private string name;
        private int level;
        private SaveDataObject.DungeonsData.DungeonData dungeon;

        public int Level => level;

        public string Name => name;

        public int Seed => seed;
        public BotSpawner MobSpawner => botSpawner;
        public bool IsCompleted => dungeon == null ? false : dungeon.IsBossDead;

        public void Init(int seed, int level, string name, SaveDataObject saveDataObject)
        {
            this.level = level;
            this.name = name;
            this.seed = seed;
            cryptNotCompletedVisualsPrefab.gameObject.SetActive(true);
            cryptCompletedVisualsPrefab.gameObject.SetActive(false);
            dungeon = saveDataObject.Dungeons.GetDungeonBySeed(seed);
            

            botSpawner.SetMobType(mobTypes.GetRandomItem(new System.Random(this.seed)));

            StartCoroutine(SkipFrame());
        }

        IEnumerator SkipFrame()
        {
            yield return null;
            if (dungeon != null)
            {
                if (dungeon.IsBossDead)
                {
                    cryptNotCompletedVisualsPrefab.gameObject.SetActive(false);
                    cryptCompletedVisualsPrefab.gameObject.SetActive(true);
                    for (int i = 0; i < guards.Length; i++)
                    {
                        guards[i].transform.eulerAngles = new Vector3(0, guards[i].transform.eulerAngles.y, 0);
                    }
                }
            }
        }
    }
}
