using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Services;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionIdle : StateAction<DungeonMob>
    {
        [SerializeField] private Range range;
        private float timer;
        private float nextTime;
        private bool waitForEnd;

        public override void ResetState()
        {
            base.ResetState();

            waitForEnd = false;
            timer = 0;
        }

        public override void StartState()
        {
            base.StartState();
            Machine.MobAnimator.StopMove();
            nextTime = range.RandomWithin();
        }

        public override void ProcessState()
        {
            timer += TimeService.DeltaTime;

            if (Machine.AIManager.IsMoving())
            {
                Machine.MobAnimator.StartMove();
            }
            else
            {
                Machine.MobAnimator.StopMove();
            }
            
            if (timer >= nextTime && !waitForEnd)
            {
                Machine.MoveToRandomPoint();

                waitForEnd = true;
                
                timer = 0;
            }
            
            if (Machine.AIManager.IsArrived() && Machine.AIManager.HavePath() && waitForEnd)
            {
                EndState();
            }
            else
            {
                if (timer >= 10)
                {
                    EndState();
                }
            }
        }
    }
}
