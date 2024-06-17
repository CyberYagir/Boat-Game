using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Map
{
    public class MapIsland : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int seed;
        [SerializeField] private Renderer renderer;

        [SerializeField] private IslandSeedData islandData;
        
        public Renderer Renderer => renderer;

        public IslandSeedData GeneratedData => islandData;

        public int Seed => seed;

        public void Init(Vector2Int pos, int seed)
        {
            this.seed = seed;
            islandData = IslandSeedData.Generate(pos);
        }
    }
}
