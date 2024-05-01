using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class RaftBase : DamageObject
    {
        [SerializeField, ReadOnly] private Vector3Int coords;
        [SerializeField] private RaftBuildService.RaftItem.ERaftType raftType;
        [SerializeField, ReadOnly] private string uid;
        

        public Vector3Int Coords => coords;
        public RaftBuildService.RaftItem.ERaftType RaftType => raftType;

        public string Uid => uid;

        public virtual void Init()
        {
            SetHealth();
            uid = Guid.NewGuid().ToString();

            OnDamage += TryStartHealthRegeneration;
        }

        private void TryStartHealthRegeneration(float obj)
        {
            StopAllCoroutines();
            timer = 0;
            StartCoroutine(RaftRegeneration());
        }

        private float healCooldown = 2;
        private float timer;
        IEnumerator RaftRegeneration()
        {
            while (Health < MaxHealth && !IsDead)
            {
                timer += TimeService.DeltaTime;
                if (timer >= healCooldown)
                {
                    SetHealth(Health + 1);
                    timer = 0;
                }
                yield return null;
            }
            
        }
        
        public void SetCoords(Vector3Int coords)
        {
            this.coords = coords;
        }

        
        public void LoadData(float raftHealth, string id)
        {
            SetHealth(raftHealth);
            if (Health < MaxHealth)
            {
                TryStartHealthRegeneration(0);
            }
            uid = id;
        }
    }
}
