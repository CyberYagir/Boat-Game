using System;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using Content.Scripts.Mobs.MobCrab;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.Mobs.Mob
{
    public class SpawnedMob : DamageObject
    {
        [SerializeField, FoldoutGroup("Settings")] private MobAnimator mobAnimator;
        
        [SerializeField, FoldoutGroup("References")] private DropTableObject dropTable;
        [SerializeField, FoldoutGroup("References")] private MobStateMachine stateMachine;
        [SerializeField, FoldoutGroup("References")] private Collider collider;
        [SerializeField, FoldoutGroup("References")] private ParticleSystem deadPoofParticles;
        
        
        [SerializeField, FoldoutGroup("Data")] protected float speed;

        private bool isMomentalDeath = false;
        public event Action<SpawnedMob> OnSpawnDrop;
        
        protected Quaternion targetQuaternion;

        protected BotSpawner spawner;
        protected bool isAttacked = false;

        public BotSpawner Spawner => spawner;

        public MobAnimator Animations => mobAnimator;
        public bool IsAttacked => isAttacked;

        public DropTableObject MobDropTable => dropTable;

        public MobStateMachine StateMachine => stateMachine;

        public bool IsMomentalDeath => isMomentalDeath;


        public virtual void Init(BotSpawner botSpawner, bool initStateMachine = true)
        {
            spawner = botSpawner;
            SetHealth();

            if (initStateMachine)
            {
                StateMachine.Init(this);
            }

            OnAttackedStart += OnOnAttackedStart;
            OnAttackedEnd += OnOnAttackedEnd;
            OnDamage += OnOnDamage;
            OnDeath += OnOnDeath;
        }

        public void SetIsMomentalDeath(bool state) => isMomentalDeath = state;

        public virtual void OnOnAttackedEnd()
        {
            isAttacked = false;
        }

        public virtual void OnOnAttackedStart()
        {
            isAttacked = true;
        }
        
        public virtual void OnOnDamage(float obj)
        {
            ChangeStateTo(EMobsState.TakeDamage);
        }
        
        private void OnOnDeath(DamageObject obj)
        {
            StateMachine.enabled = false;
            Animations.TriggerDeath();
            collider.enabled = false;

            if (!isMomentalDeath)
            {
                DOVirtual.DelayedCall(2f, delegate
                {
                    OnSpawnDrop?.Invoke(this);
                    if (spawner)
                    {
                        spawner.RespawnByCooldown();
                        gameObject.SetActive(false);
                        deadPoofParticles.transform.parent = null;
                        deadPoofParticles.Play(true);
                        Destroy(deadPoofParticles.gameObject, 2f);
                    }
                }).SetLink(gameObject).onComplete += delegate { Destroy(gameObject, 2f); };
            }
            else
            {
                OnSpawnDrop?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public void ChangeStateTo(EMobsState pointsMove)
        {
            StateMachine.StartAction(pointsMove);
        }
        
        public virtual void MoveToPoint(Vector3 lastPoint)
        {
        }
        
        protected void BaseMovement(Vector3 lastPoint, int groundMask)
        {
            if (IsAttacked) return;

            Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lastPoint - transform.position), 10f * TimeService.DeltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);


            var pos = Vector3.MoveTowards(transform.position, lastPoint, speed * TimeService.DeltaTime);
            pos.y = hit.point.y;

            transform.position = pos;
        }
    }
}