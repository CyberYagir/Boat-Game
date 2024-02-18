using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class ObjectsSO : ScriptableObject
    {
        
        [SerializeField] protected List<GameObject> prefabs;
        [SerializeField] protected List<TerrainLayer> activeLayers;
        [SerializeField] protected AnimationCurve scaleRange;
        [SerializeField] protected float scalePower = 1f;
        [SerializeField] private float maxAngle = 50;

        public float MaxAngle => maxAngle;        
        public int Count => prefabs.Count;


        public bool IsCanPlace(TerrainLayer textureLayer)
        {
            return activeLayers.Contains(textureLayer);
        }


        public float GetRandomScale(System.Random rnd)
        {
            return scaleRange.Evaluate((float)rnd.NextDouble()) * scalePower;
        }
    }
}