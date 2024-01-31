using System;
using System.Collections.Generic;
using Content.Scripts.Global;
using PathCreation;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapSpawnerService : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        [SerializeField] private GameObject islandPrefab;
        [SerializeField] private List<Texture2D> textures;
        [SerializeField] private PathCreator pathCreator;
        private double startTime;

        public double StartTime => startTime;

        public VertexPath Path => pathCreator.path;
        
        
        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameDataObject)
        {
            for (int i = 0; i < saveData.Map.Islands.Count; i++)
            {
                var il = saveData.Map.Islands[i];
                var random = new System.Random(il.IslandSeed);
                Instantiate(islandPrefab, new Vector3(il.IslandPos.x, 0, il.IslandPos.y), Quaternion.identity)
                    .With(x => x.GetComponentInChildren<Renderer>().material.SetTexture(MainTex, textures.GetRandomItem(random)))
                    .With(x => x.transform.SetYEulerAngles(random.Next(0, 360)));
            }

            var worldRandom = new System.Random(saveData.Map.WorldSeed);
            var targetPath = gameDataObject.MapPaths.GetRandomItem(worldRandom);
            startTime = worldRandom.NextDouble();
            
            pathCreator.bezierPath = new BezierPath(targetPath.PathPoints, true);
            pathCreator.TriggerPathUpdate();
            
        }
    }
}
