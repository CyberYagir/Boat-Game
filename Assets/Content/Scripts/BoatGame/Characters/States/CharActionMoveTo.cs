using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionMoveTo : CharActionBase
    {
        public override void StartState()
        {
            base.StartState();

            Agent.SetStopped(false);


            if (!MoveToPoint(GetPointToMove()))
            {
                EndState();
            }
        }

        public virtual Vector3 GetPointToMove() => SelectionService.LastWorldClick;

        public override void ProcessState()
        {
            MovingToPointLogic();
        }

        protected override void OnMoveEnded()
        {
            base.OnMoveEnded();
            EndState();
        }

        public override void EndState()
        {
            base.EndState();

            // Agent.SetStopped(true);
        }
    }
}
