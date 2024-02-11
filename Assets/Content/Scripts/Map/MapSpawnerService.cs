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

        [SerializeField] private Camera camera;
        [SerializeField] private MapIsland islandPrefab;
        [SerializeField] private List<Texture2D> textures;
        [SerializeField] private PathCreator pathCreator;

        private List<MapIsland> islands = new List<MapIsland>();

        private double startTime;

        public double StartTime => startTime;

        public VertexPath Path => pathCreator.path;

        public List<MapIsland> Islands => islands;

        public Camera Camera => camera;


        private const float yWorldOffcet = -1000;
        
        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameDataObject)
        {
            for (int i = 0; i < saveData.Map.Islands.Count; i++)
            {
                var il = saveData.Map.Islands[i];
                var random = new System.Random(il.IslandSeed);
                Instantiate(islandPrefab, new Vector3(il.IslandPos.x, yWorldOffcet, il.IslandPos.y), Quaternion.identity)
                    .With(x => x.Renderer.material.SetTexture(MainTex, textures.GetRandomItem(random)))
                    .With(x => x.transform.SetYEulerAngles(random.Next(0, 360)))
                    .With(x => Islands.Add(x))
                    .With(x => x.Init());
            }

            var worldRandom = new System.Random(saveData.Map.WorldSeed);
            var targetPath = gameDataObject.MapPaths.GetRandomItem(worldRandom);
            var points = new List<Vector3>();

            for (int i = 0; i < targetPath.PathPoints.Count; i++)
            {
                points.Add(targetPath.PathPoints[i] + Vector3.up * yWorldOffcet);
            }
            
            startTime = worldRandom.NextDouble();
            
            pathCreator.bezierPath = new BezierPath(points, true);
            pathCreator.TriggerPathUpdate();
            
        }
    }
}
