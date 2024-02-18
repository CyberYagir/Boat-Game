using System;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class IslandData : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private int temperatureAdd;

        public int TemperatureAdd => temperatureAdd;

        public Terrain Terrain => terrain;

        private void OnValidate()
        {
            terrain = GetComponent<Terrain>();
        }
    }
}
