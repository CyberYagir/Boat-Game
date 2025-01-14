using System.Collections.Generic;
using Content.Scripts.DungeonGame.Scriptable;
using Content.Scripts.Global;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonService : MonoBehaviour
    {
        [System.Serializable]
        public class ConfigsHolder
        {
            [SerializeField] private List<DungeonConfigObject> cfg;
            [SerializeField] private Range levelsRange;

            public Range LevelsRange => levelsRange;

            public List<DungeonConfigObject> Cfg => cfg;
        }
        [SerializeField] private int seed;
        [SerializeField] private int level;
        [SerializeField] private DungeonsListObject dungeonsListObject;
        [SerializeField, ReadOnly] private DungeonConfigObject targetConfig;
        
        
        private DungeonData dungeonData;
        private SaveDataObject.DungeonsData.DungeonData dungeonSaveData;

        // public Random TargetRnd => dungeonData.Random;

        public int Seed => seed;

        public int Level => level;

        public DungeonConfigObject TargetConfig => targetConfig;

        [Inject]
        private void Construct(SaveDataObject saveDataObject)
        {
            seed = saveDataObject.Global.DungeonSeed;
            dungeonData = new DungeonData(seed);
            level = dungeonData.Level;
            print(level);
            targetConfig = dungeonsListObject.GetConfigs().Find(x=>x.LevelsRange.IsInRange(level)).Cfg.GetRandomItem(dungeonData.Random);
            targetConfig.Lightning.Apply();
            dungeonSaveData = saveDataObject.Dungeons.RegisterDungeon(seed);
        }

        public void AddDestroyedUrn(string uid) => dungeonSaveData.AddUrn(uid);
        public void AddDestroyedMob(string uid) => dungeonSaveData.AddMob(uid);

        public bool IsUrnDead(string uid)
        {
            return dungeonSaveData.HasUrn(uid);
        }

        public bool IsMobDead(string uid)
        {
            return dungeonSaveData.HasMob(uid);
        }

        public void SetMobsCount(int mobsListCount)
        {
            dungeonSaveData.SetMobsCount(mobsListCount);
        }

        public int GetMaxMobsCount()
        {
            return dungeonSaveData.AllDungeonMobsCount;
        }

        public int GetDeadMobsCount()
        {
            return dungeonSaveData.DeadMobs;
        }

        public void SetDungeonsMobCount(int dungeonMobsCount)
        {
            dungeonSaveData.SetDungeonMobsCount(dungeonMobsCount);
        }

        public void SetBossDead()
        {
            dungeonSaveData.DefeatBoss();
        }

        public bool IsBossDead()
        {
            return dungeonSaveData.IsBossDead;
        }
    }
}
