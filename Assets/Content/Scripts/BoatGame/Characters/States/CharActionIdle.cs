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
            MoveToPoint(Machine.NavigationManager.WalkToAnyPoint());
            timer = idleTime.RandomWithin();
        }

        public override void ProcessState()
        {
            if (Machine.NavigationManager.NavMeshAgent.IsArrived())
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
