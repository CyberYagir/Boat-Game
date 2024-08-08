using System;
using Content.Scripts.BoatGame.Characters.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.DungeonGame.Characters.States
{
    public class Dungeon_CharActionMoveTo : Dungeon_CharActionBase
    {
        public override void StartState()
        {
            base.StartState();

            Agent.SetStopped(false);


            if (!MoveToPoint(GetPointToMove()))
            {
                EndState();
            }
        }

        public virtual Vector3 GetPointToMove() => SelectionService.LastPoint + Random.insideUnitSphere;

        public override void ProcessState()
        {
            MovingToPointLogic();
        }

        protected override void OnMoveEnded()
        {
            base.OnMoveEnded();
            EndState();
        }

        public override void EndState()
        {
            base.EndState();

            Agent.SetStopped(true);
        }
    }
}
