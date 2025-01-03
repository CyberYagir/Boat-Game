﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    [CreateAssetMenu(menuName = "Create DetailsSO", fileName = "DetailsSO", order = 0)]
    [System.Serializable]
    public class DetailsSO : ObjectsSO
    {
        [SerializeField] protected List<float> density;
        [SerializeField] private int densityScale = 1024;

        public int DensityScale => densityScale;

        public List<DetailPrototype> GetDetailPrototypes(Random rnd)
        {
            List<DetailPrototype> prototypes = new List<DetailPrototype>();

            for (int i = 0; i < weightedPrefabs.Count; i++)
            {
                prototypes.Add(new DetailPrototype()
                {
                    prototype = weightedPrefabs[i].Prefab,
                    prototypeTexture = null,
                    alignToGround = 0,
                    maxHeight = scaleRange.Evaluate(1f) * scalePower,
                    maxWidth = scaleRange.Evaluate(1f) * scalePower,
                    minWidth = scaleRange.Evaluate(0f) * scalePower,
                    minHeight = scaleRange.Evaluate(0f) * scalePower,
                    noiseSeed = rnd.Next(0, 999999),
                    noiseSpread = (float) rnd.NextDouble() * 0.2f,
                    density = density[i],
                    useInstancing = true,
                    useDensityScaling = true,
                    usePrototypeMesh = true,
                    renderMode = DetailRenderMode.VertexLit
                });
            }

            return prototypes;
        }
    }
}