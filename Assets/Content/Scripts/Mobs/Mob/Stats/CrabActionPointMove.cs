using Content.Scripts.BoatGame.Characters;
using Content.Scripts.Mobs.MobCrab;
using UnityEngine;

namespace Content.Scripts.Mobs.Mob.Stats
{
    public class CrabActionPointMove : StateAction<SpawnedMob>
    {
        [SerializeField] private float minDist = 0.25f;

        private Vector3 lastPoint;

        public override void StartState()
        {
            base.StartState();
            GetLastPoint();
            if (!Machine.IsAttacked)
            {
                StartMove();
            }else{
                Machine.Animations.StopMove();
                Machine.OnAttackedEnd += StartMove;
            }
        }

        private void StartMove()
        {
            Machine.Animations.StartMove();
            Machine.OnAttackedEnd -= StartMove;
        }


        public override void ProcessState()
        {
            Machine.MoveToPoint(lastPoint);
            if (Machine.transform.position.ToDistance(lastPoint) < minDist)
            {
                EndState();
            }
        }

        public override void EndState()
        {
            base.EndState();
            Machine.Animations.StopMove();
            Machine.OnAttackedEnd -= StartMove;
        }

        public void GetLastPoint()
        {
            lastPoint = Machine.Spawner.GetRandomPointInRange();
        }
    }
}
