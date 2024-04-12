using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Mobs;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.MobCrab
{
    public partial class Mob_Crab : SpawnedMob
    {
        [SerializeField] private CrabAnimator crabAnimator;
        [SerializeField] private CrabStateMachine stateMachine;
        [SerializeField] private Collider collider;
        [SerializeField] private ParticleSystem deadPoofParticles;
        [SerializeField] private float speed;

        public CrabAnimator Animations => crabAnimator;
        
        private int groundMask;
        private Quaternion targetQuaternion;
        public override void Init(BotSpawner botSpawner)
        {
            base.Init(botSpawner);
            groundMask = LayerMask.GetMask("Default");
            stateMachine.Init(this);
            
            OnAttackedStart += OnOnAttackedStart;
            OnAttackedEnd += OnOnAttackedEnd;
            OnDamage += OnOnDamage;
            OnDeath += OnOnDeath;
        }

        private void OnOnDeath(DamageObject obj)
        {
            stateMachine.enabled = false;
            crabAnimator.TriggerDeath();
            collider.enabled = false;

            DOVirtual.DelayedCall(2f, delegate
            {
                var item = MobDropTable.GetItem();
                spawner.SpawnItem(item);
                spawner.RespawnByCooldown();
                gameObject.SetActive(false);
                deadPoofParticles.transform.parent = null;
                deadPoofParticles.Play(true);
                Destroy(deadPoofParticles.gameObject, 2f);
            }).onComplete += delegate
            {
                Destroy(gameObject, 2f);
            };
        }

        private void OnOnDamage(float obj)
        {
            ChangeStateTo(ECrabState.TakeDamage);
        }

        private void OnOnAttackedEnd()
        {
            ChangeStateTo(ECrabState.Idle);
        }

        private void OnOnAttackedStart()
        {
            crabAnimator.StopMove();
        }

        public void MoveToPoint(Vector3 lastPoint)
        {
            if (IsAttacked) return;
            
            transform.position = Vector3.MoveTowards(transform.position, lastPoint, speed * TimeService.DeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, speed * TimeService.DeltaTime);
            
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                transform.SetYPosition(hit.point.y);
                var old = transform.rotation;
                transform.LookAt(lastPoint);
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                targetQuaternion = transform.rotation;
                transform.rotation = old;
            }
        }

        public void ChangeStateTo(ECrabState pointsMove)
        {
            stateMachine.StartAction(pointsMove);
        }
    }
}
