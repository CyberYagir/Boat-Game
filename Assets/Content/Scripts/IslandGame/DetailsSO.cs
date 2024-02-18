using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    [CreateAssetMenu(menuName = "Create DetailsSO", fileName = "DetailsSO", order = 0)]
    [System.Serializable]
    public class DetailsSO : ObjectsSO
    {
        [SerializeField] private List<float> density;


        public List<DetailPrototype> GetDetailPrototypes(Random rnd)
        {
            List<DetailPrototype> prototypes = new List<DetailPrototype>();

            for (int i = 0; i < prefabs.Count; i++)
            {
                prototypes.Add(new DetailPrototype()
                {
                    prototype = prefabs[i],
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

        public bool IsHaveTarget(GameObject prototype)
        {
            return prefabs.Contains(prototype);
        }
    }
}