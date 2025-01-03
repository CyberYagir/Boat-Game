using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mobs.Mob
{
    [System.Serializable]
    public class MobAnimator
    {
            
        private static readonly int TG_Damage = Animator.StringToHash("TG_Damage");
        private static readonly int TG_Dead = Animator.StringToHash("TG_Dead");
        private static readonly int TG_Attack = Animator.StringToHash("TG_Attack");
        private static readonly int TG_GetHit = Animator.StringToHash("TG_GetHit");
        private static readonly int TG_Sit = Animator.StringToHash("TG_Sit");
        private static readonly int TG_Idle = Animator.StringToHash("TG_Idle");
        private static readonly int TG_Drink = Animator.StringToHash("TG_Drink");
        private static readonly int TG_Cast = Animator.StringToHash("TG_Cast");
        private static readonly int Moving = Animator.StringToHash("Moving");
            
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem damageParticles;
        [SerializeField] private MobAnimationEvents animationEvents;
        [SerializeField] private float animationTransitionTime = 0.25f;

        private float moving = 0;
        private Tweener tweener;
        private static readonly int AttackType = Animator.StringToHash("AttackType");

        public MobAnimationEvents AnimationEvents => animationEvents;

        public void TriggerDamage()
        {
            animator.ResetTrigger(TG_Damage);
            animator.SetTrigger(TG_Damage);
            animator.SetFloat(Moving, 0);
            damageParticles.Play(true);
        }

        public void StartMove(bool important = false)
        {
            if (moving < 0.95f || important)
            {
                StopTweener();
                tweener = DOVirtual.Float(moving, 1f, animationTransitionTime, OnChangeMovingValue).SetLink(animator.gameObject);
            }
        }


        public void StopMove()
        {
            StopTweener();
            tweener = DOVirtual.Float(moving, 0, animationTransitionTime, OnChangeMovingValue).SetLink(animator.gameObject);
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
            moving = value;
        }

        public void TriggerDeath()
        {
            animator.SetTrigger(TG_Dead);
        }

        public void TriggerAttack()
        {
            animator.SetTrigger(TG_Attack);
        }

        public void TriggerDamageHit()
        {
            animator.ResetTrigger(TG_GetHit);
            animator.SetTrigger(TG_GetHit);
            animator.SetFloat(Moving, 0);
            damageParticles.Play(true);
        }

        public void SetMoving(float i)
        {
            StopTweener();
            OnChangeMovingValue(i);
        }


        public void TriggerSit()
        {
            animator.ResetTrigger(TG_Idle);
            animator.SetTrigger(TG_Sit);
        }
        
        public void TriggerDrink()
        {
            animator.ResetTrigger(TG_Idle);
            animator.SetTrigger(TG_Drink);
        }


        public void ResetTriggers()
        {
            animator.ResetTrigger(TG_Attack);
            animator.ResetTrigger(TG_Sit);
            animator.ResetTrigger(TG_Drink);
            animator.ResetTrigger(TG_Idle);
            animator.ResetTrigger(TG_Cast);
            animator.SetTrigger(TG_Idle);
        }

        public bool IsIdleOrMove()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsTag("IdleMove");
        }

        public void SetAttackType(int i)
        {
            animator.SetInteger(AttackType, i);
        }

        public void TriggerCast()
        {
            animator.ResetTrigger(TG_Cast);
            animator.SetTrigger(TG_Cast);
        }

        public void Disable()
        {
            animator.enabled = false;
        }
    }
}