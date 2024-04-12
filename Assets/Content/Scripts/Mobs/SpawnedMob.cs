using Content.Scripts.BoatGame;
using Content.Scripts.IslandGame.Mobs;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    public class SpawnedMob : DamageObject
    {
        [SerializeField] private DropTableObject dropTable;
        protected BotSpawner spawner;
        protected bool isAttacked = false;

        public BotSpawner Spawner => spawner;

        public bool IsAttacked => isAttacked;

        public DropTableObject MobDropTable => dropTable;

        public virtual void Init(BotSpawner botSpawner)
        {
            spawner = botSpawner;
            SetHealth();
            
            OnAttackedStart += OnOnAttackedStart;
            OnAttackedEnd += OnOnAttackedEnd;
        }

        private void OnOnAttackedEnd()
        {
            isAttacked = false;
        }

        private void OnOnAttackedStart()
        {
            isAttacked = true;
        }
    }
}