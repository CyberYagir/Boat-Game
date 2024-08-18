using Content.Scripts.BoatGame.Characters;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionTurret : DgMobActionAttackBase
    {
        [SerializeField] private Projectile projectile;
        [SerializeField] private Transform spawnPoint;
        
        
        public override void StartState()
        {
            base.StartState();
            
            Machine.MobAnimator.AnimationEvents.OnAttack += OnAttack;
            Machine.MobAnimator.SetAttackType(0);
        }

        private void OnAttack()
        {
            Instantiate(projectile, spawnPoint.position, transform.rotation)
                .With(x => x.Init(attackDamage));
        }
        
        
        public override void ProcessState()
        {
            base.ProcessState();

            if (Machine.AttackedPlayer != null)
            {
                if (!IsCanAttack(attackDistance))
                {
                    EndState();
                    return;
                }

                if (!isCooldown)
                {
                    DOVirtual.DelayedCall(attackDelay, delegate
                    {
                        if (!Machine.IsDead)
                        {
                            Machine.MobAnimator.TriggerAttack();
                            DOVirtual.DelayedCall(attackCooldown, delegate { isCooldown = false; });
                        }
                    });
                    isCooldown = true;
                }

                RotateToTarget();
            }
        }
    }
}
