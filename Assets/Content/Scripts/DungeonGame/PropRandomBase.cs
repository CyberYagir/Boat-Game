using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.DungeonGame
{
    public class PropRandomBase : MonoBehaviour
    {
        protected static List<float> notAllocatedWeights = new List<float>();

        [System.Serializable]
        public class WeightedProp
        {
            [SerializeField] private GameObject prefab;
            [SerializeField] private float weight;

            public float Weight => weight;

            public GameObject Prefab => prefab;
        }

        [SerializeField] protected List<WeightedProp> prefabs;


        public virtual void Init()
        {
            notAllocatedWeights.Clear();
            for (int i = 0; i < prefabs.Count; i++)
            {
                notAllocatedWeights.Add(prefabs[i].Weight);
            }
            
            notAllocatedWeights.RecalculateWeights();
        }
    }
}