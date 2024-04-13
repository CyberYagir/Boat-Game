using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using DG.DemiLib;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class WaterItemsSpawner : MonoBehaviour
    {
        [SerializeField] private WaterSpawnItemsObject itemsData;
        [SerializeField] private List<Transform> pointsHolders;
        [SerializeField] private int maxCount;
        [SerializeField] private int maxDistance = 40;
        [SerializeField] private Range itemSpeed = new Range(0.5f, 2f);
        [SerializeField] private float cooldown;
        
        [SerializeField] private bool drawGizmo = true;
        private float time;
        private PrefabSpawnerFabric _prefabSpawnerFabric;
        private List<WaterItem> spawnedItems = new List<WaterItem>();


        [Inject]
        private void Construct(
            TickService tickService, 
            SelectionService selectionService, 
            ScenesService scenesService, 
            GameDataObject gameDataObject, 
            PrefabSpawnerFabric prefabSpawnerFabric)
        {
            this._prefabSpawnerFabric = prefabSpawnerFabric;
            
            if (!gameObject.activeInHierarchy) return;
            
            tickService.OnTick += OnTick;
        }


        private void OnTick(float t)
        {
            if (spawnedItems.Count <= maxCount && time <= 0)
            {
                SpawnItem();
            }

            spawnedItems.RemoveAll(x => x == null);

            time -= t;
        }

        private (WaterItem, Vector3, Vector3) SpawnItem()
        {
            var (start, end) = GetPoses(pointsHolders.GetRandomIndex());
            var spawned = _prefabSpawnerFabric.SpawnItem<WaterItem>(itemsData.GetRandomItem(), start, Quaternion.identity, null)
                .With(x => x.Init(end - start, maxDistance, itemSpeed.RandomWithin()));

            spawnedItems.Add(spawned);
            time = cooldown;

            transform.SetYLocalEulerAngles(Random.Range(0, 360));

            return (spawned, start, end - start);
        }
        

        public (Vector3, Vector3) GetPoses(int id)
        {
            return (pointsHolders[id].GetChild(0).position, pointsHolders[id].GetChild(1).position);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            
            for (int i = 0; i < pointsHolders.Count; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pointsHolders[i].GetChild(0).position, pointsHolders[i].GetChild(1).position);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pointsHolders[i].GetChild(0).position, 2);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(pointsHolders[i].GetChild(1).position, 2);
            }
            
           
        }
    }
}
