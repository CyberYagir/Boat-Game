using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.IslandGame.Scriptable
{
    [CreateAssetMenu(menuName = "Create IslandGenerationDataObject", fileName = "IslandGenerationDataObject", order = 0)]
    public class IslandGenerationDataObject : ScriptableObject
    {
    
        [SerializeField] private Range islandTemperatureRange;
        [SerializeField] private IslandData[] terrains;
        [SerializeField] private TerrainBiomeSO[] biomesDatas;
        public IslandData[] Terrains => terrains;

        public Range IslandTemperatureRange => islandTemperatureRange;

        public TerrainBiomeSO[] BiomesData => biomesDatas;
    }
}
