using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackBase : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        [SerializeField] private GameObject[] particles;

        [SerializeField] private int attackIDAnimation;
        private DungeonMob dungeonMob;

        public event Action OnEnded;
        
        [Button]
        public void Init(DungeonMob dungeonMob)
        {
            this.dungeonMob = dungeonMob;
            OnUpdate(0);
            dungeonMob.MobAnimator.TriggerCast();
            
            DOVirtual.Float(0, 1, time, OnUpdate).onComplete += OnComplete;


        }

        public virtual void OnComplete()
        {
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                skinnedMeshRenderer.gameObject.SetActive(false);
            }

            foreach (var particle in particles)
            {
                particle.gameObject.SetActive(true);
            }

            if (attackIDAnimation == -1)
            {
                dungeonMob.MobAnimator.ResetTriggers();
                dungeonMob.MobAnimator.SetAttackType(attackIDAnimation);
                dungeonMob.MobAnimator.TriggerAttack();
            }
            
            
            DOVirtual.DelayedCall(0.5f, delegate
            {
                OnEnded?.Invoke();
            });
        }

        public virtual void OnUpdate(float value)
        {
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(0, value * 100);
            }
        }
    }
}