using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonEnemiesService : MonoBehaviour
    {
        [SerializeField] private List<DungeonMob> mobsList = new List<DungeonMob>();
        public event Action OnChangeEnemies;
        public event Action<DungeonMob> OnBossSpawned;
        
        private System.Random rnd;
        public int MobsCount => mobsList.Count;
        private int dungeonMobsCount;

        [Inject]
        private void Construct(DungeonService dungeonService)
        {
            this.dungeonService = dungeonService;
            rnd = new System.Random(dungeonService.Seed);
        }
        
        public void AddMob(DungeonMob mob, bool isDungeonMob)
        {
            mobsList.Add(mob);
            
            dungeonService.SetMobsCount(mobsList.Count);
            if (isDungeonMob)
            {
                dungeonMobsCount++;
                dungeonService.SetDungeonsMobCount(dungeonMobsCount);
            }
            else
            {
                OnBossSpawned?.Invoke(mob);
            }
            OnChangeEnemies?.Invoke();
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
            dungeonService.AddDestroyedMob(dungeonMob.UID);
            OnChangeEnemies?.Invoke();
        }

        private List<DungeonMob> tmpMobsList = new List<DungeonMob>(5);
        private DungeonService dungeonService;

        public List<DungeonMob> GetAllNearMobs(Vector3 transformPosition, float dist)
        {
            tmpMobsList.Clear();
            for (int i = 0; i < mobsList.Count; i++)
            {
                var btwDist = mobsList[i].transform.position.ToDistance(transformPosition);
                if (btwDist < dist)
                {
                    tmpMobsList.Add(mobsList[i]);
                }   
            }

            return tmpMobsList;
        }
        
        public string GetNextGuid()
        {
            return Extensions.GenerateSeededGuid(rnd).ToString();
        }

        public Transform GetMobByID(int i)
        {
            return mobsList[i].transform;
        }
    }
}
