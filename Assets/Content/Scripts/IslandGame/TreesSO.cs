using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    [CreateAssetMenu(menuName = "Create TreesSO", fileName = "TreesSO", order = 0)]
    [System.Serializable]
    public class TreesSO : ObjectsSO
    {
        [SerializeField] private NoiseGenerator noise;

        public NoiseGenerator Noise => noise;

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
    }
}