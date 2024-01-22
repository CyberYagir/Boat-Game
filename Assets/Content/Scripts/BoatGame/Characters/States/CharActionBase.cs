using Content.Scripts.BoatGame.Services;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionBase : StateAction<PlayerCharacter>
    {
        protected float stuckTimer = 0;
        protected NavMeshAgent Agent => Machine.NavigationManager.NavMeshAgent;
        protected SelectionService SelectionService => Machine.SelectionService;
        protected bool StuckCheck()
        {
            if (!Machine.NavigationManager.NavMeshAgent.IsArrived())
            {
                if (Machine.NavigationManager.NavMeshAgent.velocity.magnitude <= 0.001f)
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
                if (Agent.IsArrived() && Vector3.Distance(Machine.transform.position, Agent.destination) <= Agent.stoppingDistance)
                {
                    OnMoveEnded();
                }
            }
        }


        protected void MoveToPoint(Vector3 point)
        {
            if (NavMesh.SamplePosition(point,out NavMeshHit hit, Mathf.Infinity, ~0))
            {
                Agent.SetDestination(hit.position);
            }
        }

        protected virtual void OnMoveEnded()
        {
            
        }
        
        protected void ToIdleAnimation()
        {
            Machine.AnimationManager.TriggerFishingAnimation(false);
            Machine.AnimationManager.TriggerHoldFishAnimation(false);
            Machine.AnimationManager.TriggerIdle();
            Machine.NavigationManager.NavMeshAgent.velocity = Vector3.zero;
        }
    }
}