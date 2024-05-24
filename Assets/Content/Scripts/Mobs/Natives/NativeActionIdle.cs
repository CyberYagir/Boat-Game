using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.Mobs.Natives
{
    public class NativeAction : NativeActionBase
    {
        private float timer;
        private bool isMove;
        [SerializeField] private Range idleTime;


        public override void StartState()
        {
            base.StartState();

            var pos = Controller.AIManager.WalkToAnyPoint();
            MoveToPoint(pos);
            Machine.Animations.StartMove();
            timer = idleTime.RandomWithin();
            isMove = true;
        }

        public override void ProcessState()
        {
            if (Controller.AIManager.NavMeshAgent.Destination == Vector3.zero)
            {
                EndState();
                return;
            }
            if (Controller.AIManager.NavMeshAgent.IsArrived())
            {
                if (isMove)
                {
                    Machine.Animations.StopMove();
                    isMove = false;
                }

                timer -= TimeService.DeltaTime;

                if (timer <= 0)
                {
                    StartState();
                }
            }
        }
    }
}
