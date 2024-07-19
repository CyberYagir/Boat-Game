using System.Collections.Generic;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class ObjectsSO : ScriptableObject
    {
        [System.Serializable]
        public class WeightedPrefab
        {
            [SerializeField] private int weight;
            [SerializeField, PreviewField] private GameObject prefab;

            public WeightedPrefab(GameObject pr, int i)
            {
                weight = i;
                prefab = pr;
            }

            public GameObject Prefab => prefab;

            public int Weight => weight;
        }
        [SerializeField, TableList] protected List<WeightedPrefab> weightedPrefabs;
        
        [SerializeField] protected List<TerrainLayer> activeLayers;
        [SerializeField] protected AnimationCurve scaleRange;
        [SerializeField] protected float scalePower = 1f;
        [SerializeField] private float maxAngle = 50;

        public float MaxAngle => maxAngle;        
        public int Count => weightedPrefabs.Count;


        public virtual bool IsCanPlace(TerrainLayer textureLayer)
        {
            return activeLayers.Contains(textureLayer);
        }


        public float GetRandomScale(System.Random rnd)
        {
            return scaleRange.Evaluate((float)rnd.NextDouble()) * scalePower;
        }
    }
}
