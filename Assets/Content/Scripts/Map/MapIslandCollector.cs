using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Map
{
    public class MapIslandCollector : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private List<MapIsland> islandsInRadius = new List<MapIsland>(20);
        private MapSpawnerService mapSpawnerService;
        private MapMoverService mapMoverService;

        public List<MapIsland> IslandsInRadius => islandsInRadius;

        [Inject]
        private void Construct(MapSpawnerService mapSpawnerService, MapMoverService mapMoverService)
        {
            this.mapMoverService = mapMoverService;
            this.mapSpawnerService = mapSpawnerService;

            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
            while (true)
            {
                foreach (var mapIsland in mapSpawnerService.Islands)
                {
                    yield return null;

                    
                    if (Vector3.Distance(mapMoverService.Player.transform.position, mapIsland.transform.position) <= radius)
                    {
                        if (!IslandsInRadius.Contains(mapIsland))
                        {
                            IslandsInRadius.Add(mapIsland);
                        }
                    }
                    else
                    {
                        if (IslandsInRadius.Contains(mapIsland))
                        {
                            IslandsInRadius.Remove(mapIsland);
                        }
                    }
                }
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (mapMoverService == null) return;
            
            Gizmos.DrawWireSphere(mapMoverService.Player.transform.position, radius);

            for (var i = 0; i < IslandsInRadius.Count; i++)
            {
                Gizmos.DrawLine(mapMoverService.Player.transform.position, IslandsInRadius[i].transform.position);
            }
        }
    }
}
