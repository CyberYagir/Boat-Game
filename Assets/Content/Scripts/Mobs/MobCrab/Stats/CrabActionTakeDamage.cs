using System;
using Content.Scripts.BoatGame.Characters;
using DG.Tweening;
using UnityEngine;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.Mobs.MobCrab.Stats
{
    public class CrabActionTakeDamage : StateAction<Mob_Crab>
    {
        [SerializeField] private Range stanTime;
        private Tween tween;

        public override void StartState()
        {
            base.StartState();
            
            Machine.Animations.TriggerDamage();

            tween = DOVirtual.DelayedCall(stanTime.RandomWithin(), EndState);
        }

        public override void EndState()
        {
            base.EndState();
            if (tween != null)
            {
                tween.Kill();
            }
        }
    }
}
