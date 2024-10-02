using Content.Scripts.BoatGame.Services;
using Content.Scripts.Mobs.Natives;
using UnityEngine;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.Mobs.NativesEnemy
{
    public class NativeEnemyActionIdle : NativeEnemyActionBase
    {
        
        private float timer;
        private bool isMove;
        [SerializeField] private Range idleTime;
        
        

        public override void StartState()
        {
            base.StartState();
            
            
            var pos = Controller.AIManager.WalkToAnyPoint();
            Machine.Animations.StartMove(true);
            isMove = true;
            timer = idleTime.RandomWithin();
            
            
            if (!MoveToPoint(pos))
            {
                EndState();
            }

        }
        
        public override void ProcessState()
        {
            if (Controller.AIManager.NavMeshAgent.Destination == Vector3.zero)
            {
                EndState();
                return;
            }

            if (Controller.IsAttacked)
            {
                return;
            }

            if (Controller.AIManager.NavMeshAgent.IsArrived())
            {
                if (isMove)
                {
                    Machine.Animations.StopMove();
                    Machine.Animations.ResetTriggers();

                    isMove = false;
                }

                timer -= TimeService.DeltaTime;

                if (timer <= 0)
                {
                    EndState();
                }
            }
        }

        
        public override void EndState()
        {
            base.EndState();
            Machine.Animations.StopMove();
            Machine.Animations.ResetTriggers();
            
        }
    }
}
