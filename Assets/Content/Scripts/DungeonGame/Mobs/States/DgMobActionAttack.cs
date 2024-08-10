using Content.Scripts.BoatGame.Characters;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionAttack : StateAction<DungeonMob>
    {
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackDistance;
        [SerializeField] private float attackCooldown;

        private bool isCooldown;

        public override void StartState()
        {
            base.StartState();
            
            Machine.MobAnimator.AnimationEvents.OnAttack += OnAttack;
        }

        private void OnAttack()
        {
            Machine.AttackedPlayer.Damage(attackDamage, gameObject);
        }

        public override void ProcessState()
        {
            base.ProcessState();

            if (Machine.AttackedPlayer != null)
            {
                if (!IsCanAttack())
                {
                    Machine.MoveToTargetPlayer();
                    Machine.MobAnimator.StartMove();
                    return;
                }
                else
                {
                    if (!isCooldown)
                    {
                        Machine.MobAnimator.SetAttackType(2);
                        Machine.MobAnimator.TriggerAttack();
                        isCooldown = true;
                        DOVirtual.DelayedCall(attackCooldown, delegate { isCooldown = false; });
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
        
        private void RotateToTarget()
        {
            var pos = Machine.AttackedPlayer.transform.position - Machine.AttackedPlayer.transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(pos.x, Machine.transform.position.y, pos.z) - Machine.transform.position);
            Machine.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30 * Time.deltaTime);
        }

        private bool IsCanAttack()
        {
            return Vector3.Distance(transform.position, Machine.AttackedPlayer.transform.position) <= attackDistance;
        }

        public override void EndState()
        {
            base.EndState();
            
            Machine.MobAnimator.AnimationEvents.OnAttack -= OnAttack;
        }
    }
}
