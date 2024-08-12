using System;
using Content.Scripts.BoatGame.Characters.States;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame.Characters.States
{
    public class Dungeon_CharActionRoll : Dungeon_CharActionBase
    {
        [SerializeField] private float rollDistance = 3f;
        private float startSpeed = 0;
        private bool isCustomDirectionRoll;
        private Vector3 customRollPosition;
        
        public event Action OnRollEnd;

        public override void StartState()
        {
            base.StartState();

            Agent.SetStopped(false);

            var point = GetPointToMove();

            if (!Agent.TryBuildPath(point, out Vector3 newPoint))
            {
                EndState();
                return;
            }

            Vector3 dir = ((transform.position - GetNextPoint())).normalized;

            if (isCustomDirectionRoll)
            {
                dir = (customRollPosition - transform.position).normalized;
            }
            
            if (Physics.Raycast(transform.position + Vector3.up, dir, out var hit, rollDistance / 2f, LayerMask.GetMask("Default", "Terrain", "Obstacle"), QueryTriggerInteraction.Ignore))
            {
                if (!Agent.TryBuildPath(transform.position + (hit.normal * rollDistance), out newPoint))
                {
                    EndState();
                    return;
                }
            }

            transform.DOMove(new Vector3(newPoint.x, transform.position.y, newPoint.z), 0.8f).onComplete += TriggerEnd;
            transform.LookAt(new Vector3(newPoint.x, transform.position.y, newPoint.z));
            DungeonCharacter.PlayerCharacter.AnimationManager.TriggerRoll();
            DungeonCharacter.PlayerCharacter.AnimationManager.HideTorch();
            DungeonCharacter.PlayerCharacter.AIMoveManager.NavMeshAgent.SetStopped(true);
        }

        private void TriggerEnd()
        {
            OnRollEnd?.Invoke();
        }

        private void OnDrawGizmos()
        {
            if (!DungeonCharacter) return;
            Gizmos.DrawRay(transform.position + Vector3.up, ((transform.position - GetNextPoint())));
        }

        private Vector3 GetNextPoint()
        {
            if (DungeonCharacter.TargetEnemy != null)
            {
                return DungeonCharacter.TargetEnemy.transform.position;
            }
            else
            {
                return transform.position - transform.forward;
            }
        }

        public virtual Vector3 GetPointToMove(bool invert = false)
        {
            if (!isCustomDirectionRoll)
            {
                return transform.position + ((transform.position - GetNextPoint() * (invert ? -1 : 1f)).normalized * rollDistance);
            }
            else
            {
                return transform.position + ((customRollPosition - transform.position).normalized * rollDistance);
            }
        }


        public override void EndState()
        {
            base.EndState();
            DungeonCharacter.PlayerCharacter.AIMoveManager.NavMeshAgent.SetDestination(transform.position);
            DungeonCharacter.PlayerCharacter.AnimationManager.ShowTorch();
            DungeonCharacter.PlayerCharacter.AIMoveManager.NavMeshAgent.SetStopped(false);

            isCustomDirectionRoll = false;
        }

        public void SetCustomRollPosition(Vector3 position)
        {
            isCustomDirectionRoll = true;
            customRollPosition = position;
        }
    }
}
