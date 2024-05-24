using Content.Scripts.BoatGame.Characters;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.Mobs.Mob;
using UnityEngine;

namespace Content.Scripts.Mobs.Natives
{
    public class NativeActionBase : StateAction<SpawnedMob>
    {
        private Vector3 targetPoint;
        private NativeController nativeController;
        protected INavAgentProvider Agent => Controller.AIManager.NavMeshAgent;

        public NativeController Controller => nativeController;

        public override void StartState()
        {
            base.StartState();
            nativeController = Machine as NativeController;
        }

        protected bool MoveToPoint(Vector3 point)
        {
            targetPoint = point;

            if (Agent.TryBuildPath(point, out Vector3 newPoint))
            {
                Agent.SetDestination(newPoint);
                Agent.SetTargetPoint(point);
                return true;
            }

            return false;
        }
    }
}