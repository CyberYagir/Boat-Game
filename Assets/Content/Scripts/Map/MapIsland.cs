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


            public IslandData(int seed)
            {
                var rnd = new Random(seed);
                level = rnd.Next(1, 10);
            }

            public int Level => level;
        }
    
        [SerializeField, ReadOnly] private int seed;
        [SerializeField] private Renderer renderer;

        [SerializeField] private IslandData islandData;
        
        public Renderer Renderer => renderer;

        public IslandData GeneratedData => islandData;

        public void Init()
        {
            Random rnd = new Random(Mathf.FloorToInt(Mathf.Pow(transform.position.x + transform.position.z, 2) / 125f));
            seed = rnd.Next(-100000, 100000);

            islandData = new IslandData(seed);
        }
    }
}
