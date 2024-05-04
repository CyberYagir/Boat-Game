using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Mobs.MobSnake;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.Mob.Stats
{
    public class ShakeActionAttack : StateAction<SpawnedMob>
    {
        [SerializeField] private float attackTime = 2f;

        private SpawnedMobAggressive aggressiveMob;

        private float timer = 0;

        public override void ResetState()
        {
            base.ResetState();

            timer = attackTime;
        }

        public override void StartState()
        {
            base.StartState();

            aggressiveMob = Machine as SpawnedMobAggressive;

            aggressiveMob.Animations.AnimationEvents.OnAttack += AttackTargetCharacter;

        }

        private void AttackTargetCharacter()
        {
            if (aggressiveMob != null)
            {
                if (aggressiveMob.AttackedTransform != null)
                {
                    aggressiveMob.AttackTarget();
                }
            }
        }

        public override void EndState()
        {
            base.EndState();
            
            aggressiveMob.Animations.AnimationEvents.OnAttack -= AttackTargetCharacter;
        }

        public override void ProcessState()
        {
            if (aggressiveMob == null || aggressiveMob.AttackedTransform == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(aggressiveMob.AttackedTransform.position, Machine.transform.position) <= aggressiveMob.AttackRadius)
            {
                timer += TimeService.DeltaTime;
                if (timer >= attackTime)
                {
                    Machine.Animations.SetMoving(0);
                    Machine.transform.rotation = Quaternion.LookRotation(new Vector3(aggressiveMob.AttackedTransform.position.x, Machine.transform.position.y, aggressiveMob.AttackedTransform.position.z) - Machine.transform.position);
                    Machine.Animations.TriggerAttack();
                    timer = 0;
                }
            }
            else
            {
                if (!aggressiveMob.IsAttacked)
                {
                    Machine.Animations.SetMoving(1);
                    Machine.MoveToPoint(aggressiveMob.AttackedTransform.position);
                }

                timer = 0;
            }
        }
    }
}
