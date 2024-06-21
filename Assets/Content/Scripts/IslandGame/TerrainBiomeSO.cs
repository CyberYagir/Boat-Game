using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class TerrainBiomeSO : ScriptableObject
    {
        [SerializeField] private Range temperatureRange;
        [SerializeField] private Material islandMaterial;
        [SerializeField] private TerrainLayer[] layers;
        [SerializeField] private List<DetailsSO> details;
        [SerializeField] private List<TreesSO> trees;


        public TerrainLayer[] Layers => layers;
        public Range TemperatureRange => temperatureRange;
        public List<DetailsSO> DetailsData => details;
        public List<TreesSO> TreesData => trees;
        public Material IslandMaterial => islandMaterial;


        public bool isInrRange(float temp)
        {
            return (temp >= temperatureRange.min && temp < temperatureRange.max);
        }
    }
}