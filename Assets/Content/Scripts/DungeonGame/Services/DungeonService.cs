using System.Collections.Generic;
using Content.Scripts.DungeonGame.Scriptable;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.DungeonGame.Services
{
    [System.Serializable]
    public class DungeonData
    {
        [SerializeField] private int seed;
        [SerializeField] private int level;
        private Random random;

        public DungeonData(int seed)
        {
            this.seed = seed;
            random = new System.Random(seed);
            level = random.Next(1, 10);
        }

        public Random Random => random;

        public int Level => level;

        public int Seed => seed;
    }
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

        [SerializeField] private List<ConfigsHolder> configs = new List<ConfigsHolder>();

        private DungeonData dungeonData;
        [SerializeField] private int seed;
        [SerializeField] private int level;
        [SerializeField, ReadOnly] private DungeonConfigObject targetConfig;

        public Random TargetRnd => dungeonData.Random;

        public int Seed => seed;

        public int Level => level;

        public DungeonConfigObject TargetConfig => targetConfig;

        [Inject]
        private void Construct()
        {
            dungeonData = new DungeonData(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            targetConfig = configs.Find(x=>x.LevelsRange.IsInRange(level)).Cfg.GetRandomItem(TargetRnd);
        }
    }
}
