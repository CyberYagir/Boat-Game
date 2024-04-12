using Content.Scripts.BoatGame.Characters;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.MobCrab.Stats
{
    public class CrabActionIdle : StateAction<Mob_Crab>
    {
        public override void StartState()
        {
            base.StartState();
            Machine.Animations.StopMove();
            DOVirtual.DelayedCall(Random.Range(1f, 2f), delegate
            {
                Machine.ChangeStateTo(ECrabState.PointsMove);
            });
        }
    }
}
