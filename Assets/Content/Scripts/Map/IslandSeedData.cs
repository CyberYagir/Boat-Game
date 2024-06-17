using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.Map
{
    [System.Serializable]
    public class IslandSeedData
    {
        [SerializeField] private int level;
        public int Level => level;
        private IslandSeedData(Vector2Int pos)
        {
            Random rnd = new Random(Mathf.FloorToInt(Mathf.Pow((float) pos.x + pos.y, 2) / 125f));
            var seed = rnd.Next(-100000, 100000);

            rnd = new Random(seed);
            level = rnd.Next(1, 10);
        }

        public static IslandSeedData Generate(Vector2Int pos)
        {
            return new IslandSeedData(pos);
        }
        
        
    }
}