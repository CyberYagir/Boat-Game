using System;
using Content.Scripts.DungeonGame.Services;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.BossAttacks
{
    public class BossAttackBase : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        [SerializeField] private GameObject[] particles;
        [SerializeField] protected float damage;

        [SerializeField] private int attackIDAnimation;
        protected DungeonMob dungeonMob;
        protected DungeonCharactersService dungeonCharactersService;

        public event Action OnEnded;

        [Inject]
        private void Construct(DungeonCharactersService dungeonCharactersService, DungeonService dungeonService)
        {
            this.dungeonCharactersService = dungeonCharactersService;
            damage *= dungeonService.TargetConfig.DamageModify;
        }
        
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

            if (dungeonMob.IsDead)
            {
                OnEnded?.Invoke();
                return;
            }
            
            if (attackIDAnimation != -1)
            {
                dungeonMob.MobAnimator.ResetTriggers();
                dungeonMob.MobAnimator.SetAttackType(attackIDAnimation);
                dungeonMob.MobAnimator.TriggerAttack();
                print("Animation starts");
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