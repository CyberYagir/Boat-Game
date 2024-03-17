using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Content.Scripts.IslandGame.WorldStructures
{
    public class RoadBuilder : MonoBehaviour
    {
        [SerializeField] private List<Transform> nextSpawnPoints;
        [SerializeField] private List<RoadSO.ERoadsType> roadVariants;


        public List<RoadSO.ERoadsType> RoadVariants => roadVariants;

        public List<Transform> NextSpawnPoints => nextSpawnPoints;

        private void OnDrawGizmos()
        {
            if (roadVariants.Count == 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.5f);
                return;
            }
            for (int i = 0; i < nextSpawnPoints.Count; i++)
            {
                if (nextSpawnPoints[i] == null) continue;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nextSpawnPoints[i].transform.position);
                
                
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(nextSpawnPoints[i].position, 0.2f);
                Gizmos.DrawLine(nextSpawnPoints[i].position, nextSpawnPoints[i].position + nextSpawnPoints[i].forward);
            }
        }

        public void DestroyPoint(int i)
        {
            nextSpawnPoints[i].gameObject.SetActive(false);
            nextSpawnPoints[i] = null;
            
        }

        public void ShufflePoints(Random random)
        {
            nextSpawnPoints.Shuffle(random);
        }
    }
}
