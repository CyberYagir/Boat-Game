using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.MobCrab
{
    public partial class Mob_Crab
    {
        private static readonly int TG_Damage = Animator.StringToHash("TG_Damage");
        private static readonly int TG_Dead = Animator.StringToHash("TG_Dead");
        private static readonly int Moving = Animator.StringToHash("Moving");

        [System.Serializable]
        public class CrabAnimator
        {
            [SerializeField] private Animator animator;
            [SerializeField] private ParticleSystem damageParticles;
            [SerializeField] private float animationTransitionTime = 0.25f;

            private float moving = 0;
            private Tweener tweener;

            public void TriggerDamage()
            {
                animator.ResetTrigger(TG_Damage);
                animator.SetTrigger(TG_Damage);
                animator.SetFloat(Moving, 0);
                damageParticles.Play(true);
            }

            public void StartMove()
            {
                StopTweener();
                tweener = DOVirtual.Float(moving, 1f, animationTransitionTime, OnChangeMovingValue);
            }


            public void StopMove()
            {
                StopTweener();
                tweener = DOVirtual.Float(moving, 0, animationTransitionTime,
                    delegate(float value)
                    {
                        animator.SetFloat(Moving, value);
                    });
            }

            private void StopTweener()
            {
                if (tweener != null)
                {
                    tweener.Kill();
                }
            }
            
            private void OnChangeMovingValue(float value)
            {
                animator.SetFloat(Moving, value);
            }

            public void TriggerDeath()
            {
                animator.SetTrigger(TG_Dead);
            }
        }
    }
}