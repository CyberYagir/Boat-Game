using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public class PlayerAIAstar : MonoBehaviour, INavAgentProvider
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private Seeker seeker;
        [SerializeField] private Vector3 targetPoint;
        public Transform Transform => transform;
        public bool IsStopped => aiPath.isStopped;
        public bool IsOnNavMesh => true;
        public float StoppingDistance => aiPath.slowdownDistance;
        public Vector3 Velocity => GetVelocity();
        public Vector3 Destination => aiPath.destination;
        public Vector3 TargetPoint => targetPoint;

        public void SetDestination(Vector3 target)
        {
            aiPath.destination = target;
        }

        public void SetStopped(bool state)
        {
            aiPath.isStopped = state;
            aiPath.canMove = !state;
        }

        public Vector3 GetVelocity()
        {
            if (!aiPath.canMove)
            {
                return Vector3.zero;
            }

            return aiPath.desiredVelocity;
        }
        

        public bool IsArrived()
        {
            var destWithoutY = new Vector3(Destination.x, 0, Destination.z);
            var trnsWithoutY = new Vector3(Transform.position.x, 0, Transform.position.z);
            return aiPath.remainingDistance <= StoppingDistance && destWithoutY.ToDistance(trnsWithoutY) <= StoppingDistance;
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
            var constraint = NNConstraint.Default;
            constraint.constrainWalkability = true;
            constraint.walkable = true;
            constraint.constrainTags = true;
            constraint.tags = ~0;
            constraint.graphMask = seeker.graphMask;
            NNInfo info = new NNInfo();

            info = AstarPath.active.GetNearest(target, constraint);
            
            
            newPoint = info.position;
            return true;
        }

        public void Disable()
        {
            aiPath.canMove = false;
        }

        public void ChangeMask(int newMask, bool constrainInGraph)
        {
            if ((int)seeker.graphMask != newMask)
            {
                aiPath.constrainInsideGraph = constrainInGraph;
                seeker.graphMask = newMask;
                StartCoroutine(SkipFrame());
            }
        }
  
        IEnumerator SkipFrame()
        {
            yield return null;
            if (TryBuildPath(targetPoint, out Vector3 newPoint))
            {
                debugPoints.Clear();
                SetDestination(newPoint);
                SetTargetPoint(newPoint);
            }
        }
        
        public GraphMask GetCurrentGraphMask()
        {
            return seeker.graphMask;
        }

        public void SetTargetPoint(Vector3 point)
        {
            targetPoint = point;
        }

        [SerializeField] private List<Vector3> debugPoints = new List<Vector3>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Destination, 0.2f);
            if (TryBuildPath(targetPoint, out Vector3 newPoint))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(newPoint + Vector3.up * 0.4f, 0.2f);
                debugPoints.Add(newPoint);
            }   
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(TargetPoint + Vector3.up * 0.2f, 0.2f);
        }
    }
}