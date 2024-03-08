using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionBase : StateAction<PlayerCharacter>
    {
        protected float stuckTimer = 0;
        protected INavAgentProvider Agent => Machine.AIMoveManager.NavMeshAgent;
        protected PrefabSpawnerFabric Fabric => Machine.SpawnerFabric;
        protected SelectionService SelectionService => Machine.SelectionService;

        private Vector3 targetPoint;
        
        protected bool StuckCheck()
        {
            if (!Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                if (Machine.AIMoveManager.NavMeshAgent.Velocity.magnitude <= 0.001f)
                {
                    stuckTimer += TimeService.DeltaTime;

                    if (stuckTimer > 10)
                    {
                        EndState();
                    }
                }

                return true;
            }

            return false;
        }
        
        
        protected void MovingToPointLogic()
        {
            if (!StuckCheck())
            {
                MoveToPoint(targetPoint);
                if (Agent.IsArrived())
                {
                    OnMoveEnded();
                }
            }
        }


        protected bool MoveToPoint(Vector3 point)
        {
            targetPoint = point;
            if (Agent.TryBuildPath(point, out Vector3 newPoint))
            {
                Agent.SetDestination(newPoint);
                return true;
            }

            return false;
        }

        protected virtual void OnMoveEnded()
        {
            
        }
        
        protected void ToIdleAnimation()
        {
            Machine.AnimationManager.TriggerFishingAnimation(false);
            Machine.AnimationManager.TriggerHoldFishAnimation(false);
            Machine.AnimationManager.TriggerIdle();
            Machine.AIMoveManager.NavMeshAgent.SetVelocity(Vector3.zero);
        }


        private void OnDrawGizmos()
        {
            // Gizmos.DrawSphere(Agent.Destination, 0.2f);
        }
    }
}