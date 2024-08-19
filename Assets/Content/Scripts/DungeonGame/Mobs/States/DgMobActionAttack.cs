using Content.Scripts.BoatGame.Characters;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionAttack : DgMobActionAttackBase
    {
        [SerializeField] private int maxAttacks = 3;
        
        private int attackCounter;
        

        public override void ResetState()
        {
            base.ResetState();
            
            
            attackCounter = 0;
        }

        public override void StartState()
        {
            base.StartState();
            
            Machine.MobAnimator.AnimationEvents.OnAttack += OnAttack;
        }

        private void OnAttack()
        {
            attackCounter++;
            if (attackCounter >= maxAttacks)
            {
                attackCounter = 0;
            }
            
            if (Machine.AttackedPlayer.CurrentState != EStateType.Roll)
            {
                if (IsCanAttack(attackDistance))
                {
                    Machine.AttackedPlayer.Damage(attackDamage, gameObject);
                }
            }
        }

        public override void ProcessState()
        {
            base.ProcessState();

            if (Machine.AttackedPlayer != null)
            {
                if (!IsCanAttack(attackDistance))
                {
                    Machine.MoveToTargetPlayer();
                    Machine.MobAnimator.StartMove();
                    return;
                }
                else
                {
                    if (!isCooldown)
                    {
                        DOVirtual.DelayedCall(attackDelay, delegate
                        {
                            Machine.AIManager.Stop(false);
                            if (!Machine.IsDead)
                            {
                                Machine.MobAnimator.SetAttackType(attackCounter);
                                Machine.MobAnimator.TriggerAttack();
                                DOVirtual.DelayedCall(attackCooldown, delegate
                                {
                                    isCooldown = false;
                                    Machine.AIManager.Stop(true);
                                });
                            }
                        });
                        isCooldown = true;
                    }
                }

                Machine.MobAnimator.StopMove();
                RotateToTarget();
                
                
                return;
            }
            
            if (Machine.MobAnimator.IsIdleOrMove() && Machine.AIManager.IsArrived())
            {
                EndState();
            }
        }
        
        public override void EndState()
        {
            base.EndState();
            Machine.MobAnimator.AnimationEvents.OnAttack -= OnAttack;
        }
    }
}
