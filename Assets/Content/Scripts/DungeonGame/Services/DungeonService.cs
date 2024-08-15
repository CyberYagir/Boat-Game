using System.Collections.Generic;
using Content.Scripts.DungeonGame.Scriptable;
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

        [SerializeField] private List<ConfigsHolder> configs = new List<ConfigsHolder>();

        [SerializeField] private int seed;
        [SerializeField] private int level;
        [SerializeField, ReadOnly] private DungeonConfigObject targetConfig;
        private Random random;

        public Random TargetRnd => random;

        public int Seed => seed;

        public int Level => level;

        public DungeonConfigObject TargetConfig => targetConfig;

        [Inject]
        private void Construct()
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            random = new System.Random(Seed);
            level = random.Next(1, 10);
            targetConfig = configs.Find(x=>x.LevelsRange.IsInRange(level)).Cfg.GetRandomItem(random);
        }
    }
}
