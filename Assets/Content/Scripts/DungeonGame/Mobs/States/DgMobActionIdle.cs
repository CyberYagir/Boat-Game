using Content.Scripts.BoatGame.Characters;
using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs.States
{
    public class DgMobActionIdle : StateAction<DungeonMob>
    {
        public override void StartState()
        {
            base.StartState();
            Machine.MobAnimator.StopMove();
        }
    }
}
