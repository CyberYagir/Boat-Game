using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    [CreateAssetMenu(menuName = "Create TreesSO", fileName = "TreesSO", order = 0)]
    [System.Serializable]
    public class TreesSO : ObjectsSO
    {
        [SerializeField] private float treesDensity = 1;
        [SerializeField] private int minLevel;
        [SerializeField] private NoiseGenerator noise;
        public NoiseGenerator Noise => noise;

        public int MinLevel => minLevel;

        public List<TreePrototype> GetTreePrototypes()
        {
            List<TreePrototype> prototypes = new List<TreePrototype>();

            for (int i = 0; i < prefabs.Count; i++)
            {
                prototypes.Add(new TreePrototype()
                {
                    prefab = prefabs[i],
                    bendFactor = 0,
                    navMeshLod = 0
                });
            }

            return prototypes;
        }

        public bool IsDensityOk(System.Random rnd)
        {
            if (treesDensity == 1) return true;
            return rnd.NextDouble() <= treesDensity;
        }

        public GameObject GetObjectByID(int targetBiomeItem)
        {
            return prefabs[targetBiomeItem];
        }
    }
}