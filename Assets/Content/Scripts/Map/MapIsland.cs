using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.Map
{
    public class MapIsland : MonoBehaviour
    {
        [System.Serializable]
        public class IslandData
        {
            [SerializeField] private int level;
            public IslandData(Vector2Int pos)
            {
                Random rnd = new Random(Mathf.FloorToInt(Mathf.Pow((float) pos.x + pos.y, 2) / 125f));
                var seed = rnd.Next(-100000, 100000);

                rnd = new Random(seed);
                level = rnd.Next(1, 10);
            }

            public int Level => level;
        }
    
        [SerializeField, ReadOnly] private int seed;
        [SerializeField] private Renderer renderer;

        [SerializeField] private IslandData islandData;
        
        public Renderer Renderer => renderer;

        public IslandData GeneratedData => islandData;

        public int Seed => seed;

        public void Init(Vector2Int pos, int seed)
        {
            this.seed = seed;
            islandData = new IslandData(pos);
        }
    }
}
