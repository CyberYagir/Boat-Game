using Content.Scripts.BoatGame.Characters;
using Content.Scripts.Mobs.MobCrab;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.Mob.Stats
{
    public class CrabActionIdle : StateAction<SpawnedMob>
    {
        [SerializeField] private Range idleRange = new Range(1f, 2f);
        public override void StartState()
        {
            base.StartState();
            Machine.Animations.StopMove();
            DOVirtual.DelayedCall(idleRange.RandomWithin(), delegate
            {
                Machine.ChangeStateTo(EMobsState.PointsMove);
            });
        }
    }
}
