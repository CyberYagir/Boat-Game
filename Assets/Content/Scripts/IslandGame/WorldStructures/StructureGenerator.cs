using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class StructureGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class SubStructures
        {
            [System.Serializable]
            public class Variants
            {
                [SerializeField] private TerrainBiomeSO biome;
                [SerializeField] private List<Structure> variants;
            }
            
            [SerializeField] private Transform point;
            [SerializeField] private List<Variants> variants;
        }

        
        
        
    }
}
