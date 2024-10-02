using Content.Scripts.BoatGame.Services;
using Content.Scripts.Mobs.Natives;
using UnityEngine;

namespace Content.Scripts.Mobs.NativesEnemy
{
    public class NativeEnemyActionAttack : NativeEnemyActionBase
    {
        [SerializeField] private float attackTime = 2f;
        private float timer = 0;

        public override void ResetState()
        {
            base.ResetState();

            timer = attackTime;
        }

        public override void StartState()
        {
            base.StartState();


            Machine.Animations.AnimationEvents.OnAttack += AttackTargetCharacter;

        }

        private void AttackTargetCharacter()
        {
            if (Controller != null)
            {
                if (Controller.AttackedTransform != null)
                {
                    Controller.AttackTarget();
                }
            }
        }

        public override void EndState()
        {
            base.EndState();
            
            Controller.Animations.AnimationEvents.OnAttack -= AttackTargetCharacter;
        }

        bool isMove = false;
        public override void ProcessState()
        {
            if (Controller == null || Controller.AttackedTransform == null)
            {
                EndState();
                return;
            }

            if (Vector3.Distance(Controller.AttackedTransform.position, Machine.transform.position) <= Controller.AttackRadius)
            {
                timer += TimeService.DeltaTime;
                Controller.AIManager.NavMeshAgent.SetStopped(true);
                Machine.Animations.StopMove();
                if (timer >= attackTime)
                {
                    Machine.transform.rotation = Quaternion.LookRotation(new Vector3(Controller.AttackedTransform.position.x, Machine.transform.position.y, Controller.AttackedTransform.position.z) - Machine.transform.position);
                    Machine.Animations.TriggerAttack();
                    timer = 0;
                }
            }
            else
            {
                Controller.AIManager.NavMeshAgent.SetStopped(false);
                if (!Controller.IsAttacked)
                {
                    Machine.Animations.StartMove();
                    MoveToPoint(Controller.AttackedTransform.position);
                }

                timer = 0;
            }
        }
    }
}
