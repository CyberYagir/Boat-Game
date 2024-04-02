using System.Collections.Generic;
using Content.Scripts.Global;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class RandomStructureMaterialsBase : MonoBehaviour
    {
        [SerializeField] protected GameDataObject gameData;
        [SerializeField] protected Renderer[] renderer;

        [System.Serializable]
        public class MatsByBiome
        {
            [SerializeField] private List<TerrainBiomeSO> biome;
            [SerializeField] private Material material;

            public Material Material => material;

            public List<TerrainBiomeSO> Biome => biome;
        }

        public virtual void Init(TerrainBiomeSO biome)
        {
            
        }
    }
}