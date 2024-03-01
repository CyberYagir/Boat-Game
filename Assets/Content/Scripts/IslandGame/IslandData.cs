using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class IslandData : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnPoint
        {
            [SerializeField] private Transform point, ladderPoint;

            public SpawnPoint(Transform getChild, Transform getChild1)
            {
                point = getChild;
                ladderPoint = getChild1;
            }

            public Transform LadderPoint => ladderPoint;

            public Transform Point => point;
        }
        [SerializeField] private Terrain terrain;
        [SerializeField] private int temperatureAdd;
        [SerializeField] private List<SpawnPoint> spawnPoints;
        public int TemperatureAdd => temperatureAdd;

        public Terrain Terrain => terrain;

        public List<SpawnPoint> SpawnPoints => spawnPoints;

        private void OnValidate()
        {
            terrain = GetComponent<Terrain>();
        }


        [Button]
        private void GetSpawns(GameObject target)
        {
            SpawnPoints.Clear();

            for (int i = 0; i < target.transform.childCount; i++)
            {
                SpawnPoints.Add(new SpawnPoint(target.transform.GetChild(i), target.transform.GetChild(i).GetChild(0)));
            }
        }

        [Button]
        private void ClearTerrain()
        {
            terrain.terrainData.detailPrototypes = new DetailPrototype[0];
            terrain.terrainData.treePrototypes = new TreePrototype[0];
            terrain.terrainData.SetDetailResolution(512, 64);
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                if (!Application.isPlaying)
                {
                    Physics.Raycast(SpawnPoints[i].LadderPoint.position + Vector3.up * 100, Vector3.down, out RaycastHit hit);
                    SpawnPoints[i].LadderPoint.position = hit.point;
                    SpawnPoints[i].Point.position = new Vector3(SpawnPoints[i].Point.position.x, 0, SpawnPoints[i].Point.position.z);
                }

                
                Gizmos.DrawWireSphere(SpawnPoints[i].Point.position, 0.5f); 
                Gizmos.DrawSphere(SpawnPoints[i].LadderPoint.position, 0.5f); 
                Gizmos.DrawLine(SpawnPoints[i].Point.position, SpawnPoints[i].LadderPoint.position); 
            }
        }
        
    }
}
