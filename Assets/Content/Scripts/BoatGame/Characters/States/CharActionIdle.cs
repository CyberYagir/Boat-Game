using Content.Scripts.BoatGame.Services;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionIdle : CharActionBase
    {
        private float timer;
        [SerializeField] private Range idleTime;
        public override void StartState()
        {
            base.StartState();
            var pos = Machine.AIMoveManager.WalkToAnyPoint();
            MoveToPoint(pos);
            timer = idleTime.RandomWithin();
        }

        public override void ProcessState()
        {
            if (Machine.AIMoveManager.NavMeshAgent.Destination == Vector3.zero)
            {
                EndState();
                return;
            }
            if (Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                timer -= TimeService.DeltaTime;

                if (timer <= 0)
                {
                    StartState();
                }
            }
        }
    }
}
