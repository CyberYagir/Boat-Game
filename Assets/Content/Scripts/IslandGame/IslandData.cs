using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Mobs;
using Sirenix.OdinInspector;
using UnityEditor;
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

        [SerializeField] private Vector3 size;
        [SerializeField] private Vector3 offcet;
        [SerializeField] private BotSpawnersManager botsSpawnerManager;
        
        public int TemperatureAdd => temperatureAdd;

        public Terrain Terrain => terrain;

        public List<SpawnPoint> SpawnPoints => spawnPoints;

        public BotSpawnersManager BotsSpawnerManager => botsSpawnerManager;

        private void OnValidate()
        {
            terrain = GetComponent<Terrain>();
            botsSpawnerManager = GetComponentInChildren<BotSpawnersManager>();
        }

        public void Init(GameDataObject gameData, PrefabSpawnerFabric prefabSpawner)
        {
            if (botsSpawnerManager)
            {
                botsSpawnerManager.Init(gameData, prefabSpawner);
            }
        }

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

        public Bounds GetBounds()
        {
            var pos = transform.position + offcet;
            pos.y = 0;
            size.y = 500;
            
            return new Bounds(pos, size);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red * new Color(1, 1, 1, 0.2f);
            Gizmos.DrawCube(GetBounds().center, GetBounds().size);
            
            for (int i = 0; i < SpawnPoints.Count; i++)
            {
                if (!Application.isPlaying)
                {
                    Physics.Raycast(SpawnPoints[i].LadderPoint.position + Vector3.up * 100, Vector3.down, out RaycastHit hit);
                    SpawnPoints[i].LadderPoint.position = hit.point;
                    SpawnPoints[i].Point.position = new Vector3(SpawnPoints[i].Point.position.x, 0, SpawnPoints[i].Point.position.z);
                }


                Gizmos.DrawWireSphere(SpawnPoints[i].Point.position, 0.5f);
                var dist = Vector3.Distance(SpawnPoints[i].LadderPoint.position, SpawnPoints[i].Point.position);

                Gizmos.DrawCube(SpawnPoints[i].Point.position, new Vector3(2, 0.2f, 2f));

                if (dist >= (70 * 0.25f) * 0.9f || SpawnPoints[i].LadderPoint.position.y <= 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Gizmos.DrawSphere(SpawnPoints[i].LadderPoint.position, 0.5f);
                Gizmos.DrawLine(SpawnPoints[i].Point.position, SpawnPoints[i].LadderPoint.position);
            }
        }
        
        
        
    }
}
