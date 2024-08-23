using UnityEngine;
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
}