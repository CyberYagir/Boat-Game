using System;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionBase : StateAction<PlayerCharacter>
    {
        protected float stuckTimer = 0;
        protected NavMeshAgent Agent => Machine.AIMoveManager.NavMeshAgent;
        protected PrefabSpawnerFabric Fabric => Machine.SpawnerFabric;
        protected SelectionService SelectionService => Machine.SelectionService;

        private Vector3 targetDestination;
        
        protected bool StuckCheck()
        {
            if (!Machine.AIMoveManager.NavMeshAgent.IsArrived())
            {
                if (Machine.AIMoveManager.NavMeshAgent.velocity.magnitude <= 0.001f)
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
                if (Agent.IsArrived() && targetDestination.ToDistance(Agent.transform.position) <= Agent.stoppingDistance && !Agent.pathPending)
                {
                    OnMoveEnded();
                }
            }
        }


        protected bool MoveToPoint(Vector3 point)
        {
            if (NavMesh.SamplePosition(point,out NavMeshHit hit, Mathf.Infinity, ~0))
            {
                targetDestination = hit.position;
                Agent.SetDestination(hit.position);
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
            Machine.AIMoveManager.NavMeshAgent.velocity = Vector3.zero;
        }


        private void OnDrawGizmos()
        {
            // Gizmos.DrawSphere(Agent.destination, 0.2f);
        }
    }
}