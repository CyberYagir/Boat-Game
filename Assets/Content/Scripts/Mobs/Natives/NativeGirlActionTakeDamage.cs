using Content.Scripts.BoatGame.Services;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.Natives
{
    public class NativeGirlActionTakeDamage : NativeActionBase
    {
        [SerializeField] private Range stanTime;
        [SerializeField] private Range socialRatingRemove;
        private Tween tween;

        public override void StartState()
        {
            base.StartState();

            Machine.Animations.TriggerDamage();
            var minusRating = (int) socialRatingRemove.RandomWithin();
            Controller.VillageData.RemoveSocialRating(minusRating);
            WorldPopupService.StaticSpawnPopup(transform.position, (-minusRating) + "<sprite=4>");
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