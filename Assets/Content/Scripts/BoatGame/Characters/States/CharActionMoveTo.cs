namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionMoveTo : CharActionBase
    {
        public override void StartState()
        {
            base.StartState();
            
            Machine.AIMoveManager.NavMeshAgent.isStopped = false;
            
            
            MoveToPoint(SelectionService.LastWorldClick);
        }

        public override void ProcessState()
        {
            print(Agent.pathPending + " " + Agent.pathStatus + " " + Agent.hasPath);
            if (Agent.pathPending) return;
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
