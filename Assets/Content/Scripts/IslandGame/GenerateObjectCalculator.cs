using System.Collections.Generic;
using System.Linq;
using Content.Scripts.IslandGame.WorldStructures;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame
{
    [System.Serializable]
    public class GenerateObjectCalculator
    {
        protected IslandGenerator islandGenerator;
        [SerializeField] protected int accuracy = 50;
        [SerializeField] protected float maxGap = 5;
        [SerializeField] protected float maxAngle = 15;
        [SerializeField] protected float minY = 1f;
        [SerializeField] protected float terrainBorder = 0.1f;

        public float TerrainBorder => terrainBorder;

        public float MinY => minY;

        public float MaxAngle => maxAngle;

        public float MaxGap => maxGap;

        public int Accuracy => accuracy;

        public static void DrawDebugIndicator(Transform village, Color color)
        {
#if UNITY_EDITOR
            var end = village.position + Vector3.down * 500;
            end.y = 0;
            Debug.DrawLine(village.position + Vector3.up * 50, end, color, 5);
#endif
        }

        public void RandomPos(Random rnd, Transform village, Bounds startSize)
        {
            village.position = new Vector3(rnd.Next((int) startSize.min.x, (int) startSize.max.x), 0, rnd.Next((int) startSize.min.z, (int) startSize.max.z));
        }

        public bool CalculateSurface(Transform spawned, List<TerrainLayer> allowedBiomes, TerrainBiomeSO biome, out Vector3 point)
        {
            var targetTerrain = islandGenerator.TargetTerrain.Terrain;
            
            var startSize = new Vector3(targetTerrain.terrainData.size.x, 0, targetTerrain.terrainData.size.z);
            
            var pos = targetTerrain.transform.InverseTransformPoint(spawned.position);

            var onTerrainPos = pos.DevideVector3(startSize);

            point = Vector3.zero;
            
            if (onTerrainPos.x > 1f - terrainBorder || onTerrainPos.x < terrainBorder || onTerrainPos.z > 1f - terrainBorder || onTerrainPos.z < terrainBorder)
            {
                return false;
            }

            if (!Physics.Raycast(spawned.transform.position + Vector3.up * 500, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Terrain")))
            {
                DrawDebugIndicator(spawned, Color.red);
                return false;
            }

            if (hit.point.y < minY)
            {
                DrawDebugIndicator(spawned, Color.blue);
                return false;
            }


            point = hit.point;
            
            var layer = islandGenerator.GetTextureLayerID(targetTerrain.terrainData, onTerrainPos.x, onTerrainPos.z, biome, out int i);
            if (!allowedBiomes.Contains(layer))
            {
                DrawDebugIndicator(spawned, Color.magenta);
                return false;
            }

            if (islandGenerator.GetAngle(
                (int) (onTerrainPos.x * targetTerrain.terrainData.detailResolution),
                (int) (onTerrainPos.z * targetTerrain.terrainData.detailResolution)) > maxAngle)
            {
                DrawDebugIndicator(spawned, Color.yellow);
                return false;
            }

            return true;
        }

        public void SetIslandGenerator(IslandGenerator islandGenerator)
        {
            this.islandGenerator = islandGenerator;
        }
        
        
        public bool IsGapAvailable(List<Vector3> points)
        {
            int maxCalGap = 0;
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    var delta = Mathf.Abs(points[i].y - points[j].y);
                    if (delta > maxCalGap)
                    {
                        maxCalGap = (int) delta;

                        if (maxCalGap > maxGap)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}