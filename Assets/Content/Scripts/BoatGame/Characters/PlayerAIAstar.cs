using System;
using System.Linq;
using Pathfinding;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public class PlayerAIAstar : MonoBehaviour, INavAgentProvider
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private Seeker seeker;

        public Transform Transform => transform;
        public bool IsStopped => aiPath.isStopped;
        public bool IsOnNavMesh => true;
        public float StoppingDistance => aiPath.slowdownDistance;
        public Vector3 Velocity => aiPath.desiredVelocity;
        public Vector3 Destination => aiPath.destination;

        public void SetDestination(Vector3 target)
        {
            aiPath.destination = target;
        }

        public void SetStopped(bool state)
        {
            aiPath.isStopped = state;
        }

        public bool IsArrived()
        {
            return aiPath.remainingDistance <= StoppingDistance && Transform.position.ToDistance(Destination) <= StoppingDistance;
        }

        public void ExtraRotation()
        {
        }

        public void SetVelocity(Vector3 newVel)
        {
            SetStopped(newVel == Vector3.zero);
        }

        public bool TryBuildPath(Vector3 target, out Vector3 newPoint)
        {
            var constraint = NNConstraint.None;
            constraint.constrainWalkability = true;
            constraint.walkable = true;
            constraint.constrainTags = true;
            constraint.tags = ~0;
            NNInfo info = new NNInfo();
            for (int i = 0; i < AstarPath.active.graphs.Length; i++)
            {
                info = AstarPath.active.GetNearest(target, constraint);
            }

            newPoint = info.position;
            return true;
        }

        public void Disable()
        {
            aiPath.canMove = false;
        }
    }
}