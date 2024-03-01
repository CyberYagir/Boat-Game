using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionMoveTo : CharActionBase
    {
        public override void StartState()
        {
            base.StartState();
            
            Machine.AIMoveManager.NavMeshAgent.isStopped = false;


            if (!MoveToPoint(SelectionService.LastWorldClick))
            {
                EndState();
            }
            else
            {
                print("move to point");
            }
        }

        public override void ProcessState()
        {
            print(Agent.pathStatus);
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
