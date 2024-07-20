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

            for (int i = 0; i < weightedPrefabs.Count; i++)
            {
                prototypes.Add(new TreePrototype()
                {
                    prefab = weightedPrefabs[i].Prefab,
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
            return weightedPrefabs[targetBiomeItem].Prefab;
        }

        private List<float> weights = new List<float>();

        public void PrepareWeights()
        {
            if (weights.Count == 0)
            {
                for (int i = 0; i < weightedPrefabs.Count; i++)
                {
                    weights.Add(weightedPrefabs[i].Weight);
                }
                
                weights.RecalculateWeights();
            }
        }

        public int GetRandomTreeIndexByWeight(System.Random rnd) => weights.ChooseRandomIndexFromWeights(rnd);
    }
}