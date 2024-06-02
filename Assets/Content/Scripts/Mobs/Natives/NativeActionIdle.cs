using System;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.Mobs.Natives
{
    public class NativeActionIdle : NativeActionBase
    {
        public enum EIdleSubState
        {
            GoToPoint,
            GoToSeat,
        }
        private float timer;
        private bool isMove;
        private EIdleSubState subState;
        [SerializeField] private Range idleTime;
        
        

        public override void StartState()
        {
            base.StartState();

            subState = EIdleSubState.GoToPoint;

            Vector3 pos = Vector3.zero;
            switch (subState)
            { 
                case EIdleSubState.GoToPoint:
                    pos = Controller.AIManager.WalkToAnyPoint();
                    break;
                case EIdleSubState.GoToSeat:
                    pos = Controller.VillageData.GetRandomAvailableSit().transform.position;
                    Controller.AIManager.IsAvailablePoint(pos);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
