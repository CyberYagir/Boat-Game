namespace Content.Scripts.Mobs.Natives
{
    public class NativeActionStay : NativeActionBase
    {
        public override void StartState()
        {
            base.StartState();
            Controller.AIManager.NavMeshAgent.SetStopped(true);
            Controller.Animations.StopMove();
        }
    }
}
