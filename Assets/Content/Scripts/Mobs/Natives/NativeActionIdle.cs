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
            GoToSeat
        }
        private float timer;
        private bool isMove;
        private EIdleSubState subState;
        [SerializeField] private Range idleTime;
        
        

        public override void StartState()
        {
            base.StartState();

            subState = Extensions.GetRandomEnum<EIdleSubState>();
            Vector3 pos = Vector3.zero;
            switch (subState)
            { 
                case EIdleSubState.GoToPoint:
                    pos = Controller.AIManager.WalkToAnyPoint();
                    break;
                case EIdleSubState.GoToSeat:
                    var sit = Controller.VillageData.GetRandomAvailableSit();
                    if (sit)
                    {
                        pos = Controller.VillageData.GetRandomAvailableSit().transform.position;
                        Controller.AIManager.IsAvailablePoint(pos);
                    }
                    else
                    {
                        pos = Controller.AIManager.WalkToAnyPoint();
                    }

                    break;
            }

            if (!MoveToPoint(pos))
            {
                EndState();
                return;
            }
            
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
                    if (subState == EIdleSubState.GoToSeat)
                    {
                        Controller.Animations.TriggerSit();
                    }
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
