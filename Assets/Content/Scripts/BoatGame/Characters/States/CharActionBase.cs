using System;
using Content.Scripts.BoatGame.Services;
using Pathfinding;
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

        private bool waitForMoveFromRaft;
        
        
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
            if (Machine.AIMoveManager.NavMesh.GetGraphsCount() > 1)
            {
                var isOnRaftGraph = IsOnRaftGraph();
                CheckToGraphSwitchOnTerrain(isOnRaftGraph);
            }

            if (!StuckCheck())
            {
                MoveToPoint(targetPoint);
                if (Agent.IsArrived())
                {
                    OnMoveEnded();
                }
            }
        }


        private Vector3 tempPoint;
        protected bool MoveToPoint(Vector3 point)
        {
            targetPoint = point;
            Agent.SetArrivePoint(point);

            if (AddMoveFromRaftSubPath()) return true;
            
            
            if (Agent.TryBuildPath(point, out Vector3 newPoint))
            {
                Agent.SetDestination(newPoint);
                Agent.SetTargetPoint(point);
                return true;
            }

            return false;
        }

        private bool AddMoveFromRaftSubPath()
        {
            var terrainMask = LayerMask.GetMask("Terrain", "Raft", "Water");
            var offset = Vector3.up * 50;
            
            bool IsPlayerNotOnTerrain(out RaycastHit hit)
            {
                if (Physics.Raycast(transform.position + offset, Vector3.down, out hit, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore))
                {
                    if (!hit.collider.GetComponent<Terrain>())
                    {
                        return true;
                    }
                }

                return false;
            }
            
            
            if (Machine.AIMoveManager.NavMesh.GetGraphsCount() > 1)
            {
                var isOnRaftGraph = IsOnRaftGraph();


                if (!waitForMoveFromRaft && isOnRaftGraph)
                {
                    var navMesh = Machine.AIMoveManager.NavMesh;
    
                    var terrainPoint = navMesh.GetNavMeshByID(0).GetNearest(targetPoint, NNConstraint.Walkable).position;
                    var raftPoint =  navMesh.GetNavMeshByID(1).GetNearest(targetPoint, NNConstraint.Walkable).position;

                    if (Vector3.Distance(targetPoint, terrainPoint) > Vector3.Distance(targetPoint, raftPoint)) return false;
                    
                    
                    if (Machine.BuildService.RaftEndPoint != null)
                    {
                        if (IsPlayerNotOnTerrain(out RaycastHit hit))
                        {
                            if (Physics.Raycast(targetPoint + offset, Vector3.down, out hit, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore))
                            {
                                var mask = LayerMask.GetMask("Terrain", "Water");
                                if (( mask & (1 << hit.collider.transform.gameObject.layer)) != 0)
                                {
                                    var raftDelta = (Machine.BuildService.RaftEndPoint.position - Machine.BuildService.Holder.transform.position);

                                    var raftGraph = Machine.AIMoveManager.NavMesh.GetNavMeshByID(NavMeshConstants.IslandRaftGraph) as GridGraph;

                                    tempPoint =
                                        Machine.BuildService.Holder.position +
                                        raftDelta.normalized * ((raftGraph.size.magnitude * raftGraph.nodeSize) * 2f);


                                    Debug.DrawLine(tempPoint, tempPoint + Vector3.up * 100, Color.green, 5);

                                    if (Physics.Raycast(tempPoint + offset, Vector3.down, out hit, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore))
                                    {
                                        tempPoint = hit.point;
                                        Agent.SetDestination(tempPoint);
                                        Agent.SetTargetPoint(tempPoint);
                                        waitForMoveFromRaft = true;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool IsOnRaftGraph()
        {
            var navMesh = Machine.AIMoveManager.NavMesh;
            var mask = Machine.AIMoveManager.NavMeshAgent.GetCurrentGraphMask();
            bool isOnRaftGraph = mask == GraphMask.FromGraph(navMesh.GetNavMeshByID(NavMeshConstants.IslandRaftGraph));
            return isOnRaftGraph;
        }

        private bool CheckToGraphSwitchOnTerrain(bool isOnRaftGraph)
        {
            if (waitForMoveFromRaft)
            {
                Agent.SetDestination(tempPoint);
                if (!isOnRaftGraph)
                {
                    Agent.SetDestination(targetPoint);
                    Agent.SetTargetPoint(targetPoint);
                    waitForMoveFromRaft = false;
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnMoveEnded()
        {
            
        }
        
        protected void ToIdleAnimation()
        {
            waitForMoveFromRaft = false;
            Machine.AnimationManager.TriggerFishingAnimation(false);
            Machine.AnimationManager.TriggerHoldFishAnimation(false);
            Machine.AnimationManager.TriggerIdle();
            Machine.AIMoveManager.NavMeshAgent.SetVelocity(Vector3.zero);
        }


        public virtual bool IsCanCancel()
        {
            return true;
        }
    }
}