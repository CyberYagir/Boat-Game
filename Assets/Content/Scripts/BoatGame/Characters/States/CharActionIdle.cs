using Content.Scripts.BoatGame.Services;
using DG.DemiLib;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionIdle : CharActionBase
    {
        [SerializeField] private float timer;
        [SerializeField] private Range idleTime;

        public override void StartState()
        {
            base.StartState();
            var pos = Machine.AIMoveManager.WalkToAnyPoint();
            if (!MoveToPoint(pos))
            {
                EndState();
            }
            timer = idleTime.RandomWithin();
        }

        public override void ProcessState()
        {
            if (Machine.AIMoveManager.NavMeshAgent.Destination.magnitude >= Mathf.Infinity)
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
