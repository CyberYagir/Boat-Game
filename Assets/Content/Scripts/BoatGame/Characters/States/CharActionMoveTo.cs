using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionMoveTo : CharActionBase
    {
        public override void StartState()
        {
            base.StartState();
            
            Agent.isStopped = false;


            if (!MoveToPoint(SelectionService.LastWorldClick))
            {
                EndState();
            }
        }

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

            Agent.isStopped = true;
        }
    }
}
