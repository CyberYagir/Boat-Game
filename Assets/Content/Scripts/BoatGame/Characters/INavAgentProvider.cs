using Pathfinding;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public interface INavAgentProvider
    {
        public Transform Transform { get; }
        public bool IsStopped { get; }
        public bool IsOnNavMesh { get; }
        public float StoppingDistance { get; }
        public Vector3 Velocity { get; }
        public Vector3 Destination { get; }
        public Vector3 TargetPoint { get; }
        public float MaxSpeed { get; }
        
        public void SetDestination(Vector3 target);
        public void SetStopped(bool state);
        public bool IsArrived();

        public void ExtraRotation();
        public void SetVelocity(Vector3 newVel);

        public bool TryBuildPath(Vector3 target, out Vector3 newPoint);
        
        public void Disable();

        void ChangeMask(int newMask, bool constrainInGraph);
        GraphMask GetCurrentGraphMask();
        public void SetTargetPoint(Vector3 point);

        public void SetMovingSpeed(float value);
    }
}