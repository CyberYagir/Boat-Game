using System;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.WorldStructures;
using DG.Tweening;
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
            GoToWater
        }

        private float timer;
        private bool isMove;
        private EIdleSubState subState;
        private NativesSit targetSit;
        [SerializeField] private Range idleTime;
        [SerializeField] private GameObject waterBucket;
        private Tween waterBucketTween;

        public override void ResetState()
        {
            targetSit = null;
        }

        public override void StartState()
        {
            base.StartState();

            var pos = GenerateRandomSubState();

            if (!MoveToPoint(pos))
            {
                EndState();
                return;
            }

            Machine.Animations.StartMove();
            isMove = true;
        }

        private Vector3 GenerateRandomSubState()
        {
            subState = Extensions.GetRandomEnum<EIdleSubState>();
            Vector3 pos = Vector3.zero;
            switch (subState)
            {
                case EIdleSubState.GoToPoint:
                    pos = Controller.AIManager.WalkToAnyPoint();
                    break;
                case EIdleSubState.GoToSeat:
                    var sit = Controller.VillageData.GetRandomAvailableSit();
                    pos = GoToStructure(sit);
                    break;
                case EIdleSubState.GoToWater:
                    var source = Controller.VillageData.GetRandomAvailableWaterSource();
                    pos = GoToStructure(source);
                    timer = 5f;
                    break;
            }

            return pos;
        }

        private Vector3 GoToStructure(NativesSit sit)
        {
            Vector3 pos;
            if (sit)
            {
                targetSit = sit;
                pos = sit.transform.position;
                if (Controller.AIManager.IsAvailablePoint(pos))
                {
                    targetSit.SetState(true);
                }
                else
                {
                    pos = Controller.AIManager.WalkToAnyPoint();
                }
            }
            else
            {
                pos = Controller.AIManager.WalkToAnyPoint();
            }

            timer = idleTime.RandomWithin();
            return pos;
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

                    CalculateSubStateAction();

                    isMove = false;
                }

                timer -= TimeService.DeltaTime;

                if (timer <= 0)
                {
                    EndState();
                }
            }
        }

        private void CalculateSubStateAction()
        {
            switch (subState)
            {
                case EIdleSubState.GoToPoint:
                    break;
                case EIdleSubState.GoToSeat:
                    Controller.Animations.TriggerSit();
                    break;
                case EIdleSubState.GoToWater:
                    AnimateWaterDriking();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AnimateWaterDriking()
        {
            Controller.Animations.TriggerDrink();
            waterBucket.gameObject.SetActive(true);

            DOVirtual.DelayedCall(4f, delegate { waterBucket.gameObject.SetActive(false); }).SetUpdate(UpdateType.Fixed);
        }

        public override void EndState()
        {
            base.EndState();
            Machine.Animations.StopMove();
            Machine.Animations.ResetTriggers();
            if (waterBucketTween != null)
            {
                waterBucketTween.Kill();
                waterBucketTween = null;
            }
            
            waterBucket.gameObject.SetActive(false);
            if (targetSit)
            {
                targetSit.SetState(false);
            }
        }
    }
}
