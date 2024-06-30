using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters
{
    public class PlayerAINavAgent : MonoBehaviour, INavAgentProvider
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        private INavAgentProvider _navAgentProviderImplementation;
        public Transform Transform => transform;
        public bool IsStopped => navMeshAgent.isStopped;
        public bool IsOnNavMesh => navMeshAgent.isOnNavMesh;
        public float StoppingDistance => navMeshAgent.stoppingDistance;
        public Vector3 Velocity => navMeshAgent.velocity;
        public Vector3 Destination => navMeshAgent.destination;
        public Vector3 TargetPoint { get; }
        public float MaxSpeed { get; }

        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public void SetStopped(bool state)
        {
            navMeshAgent.isStopped = state;
        }

        public bool IsArrived()
        {
            return navMeshAgent.IsArrived();
        }
        

        public void ExtraRotation()
        {
            if (IsStopped && IsOnNavMesh) return;

            Vector3 lookrotation = navMeshAgent.steeringTarget - navMeshAgent.transform.position;
            if (lookrotation != Vector3.zero)
            {
                navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, Quaternion.LookRotation(lookrotation), navMeshAgent.angularSpeed * Time.deltaTime);
            }
        }

        public void SetVelocity(Vector3 newVel)
        {
            navMeshAgent.velocity = newVel;
        }

        public bool TryBuildPath(Vector3 target, out Vector3 newPoint)
        {
            bool isCan = NavMesh.SamplePosition(target, out NavMeshHit hit, Mathf.Infinity, ~0);
            newPoint = hit.position;
            return isCan;
        }
        

        public void Disable()
        {
            navMeshAgent.enabled = false;
        }

        public void ChangeMask(int newMask, bool constrainInGraph)
        {
            throw new System.NotImplementedException();
        }

        public GraphMask GetCurrentGraphMask()
        {
            throw new System.NotImplementedException();
        }

        public void SetTargetPoint(Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public void SetMovingSpeed(float value)
        {
            throw new System.NotImplementedException();
        }
    }
}
