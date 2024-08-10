using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonEnemiesService : MonoBehaviour
    {
        [SerializeField] private List<DungeonMob> mobsList = new List<DungeonMob>();


        public void AddMob(DungeonMob mob)
        {
            mobsList.Add(mob);
        }

        public DungeonMob GetNearMob(Vector3 point)
        {
            if (mobsList.Count == 0) return null;
            if (mobsList.Count == 1) return mobsList[0];
            
            float dist = 999f;
            DungeonMob mob = null;
            for (int i = 0; i < mobsList.Count; i++)
            {
                var btwDist = mobsList[i].transform.position.ToDistance(point);
                if (btwDist < dist)
                {
                    dist = btwDist;
                    mob = mobsList[i];
                }   
            }

            return mob;
        }

        public void RemoveMob(DungeonMob dungeonMob)
        {
            mobsList.Remove(dungeonMob);
        }
    }
}
