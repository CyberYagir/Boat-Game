using System;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;
using Range = DG.DemiLib.Range;

namespace Content.Scripts.BoatGame.RaftDamagers
{
    public class RaftDamagerShark : RaftDamager
    {
        [System.Serializable]
        public class DamageSharkAnimator
        {
            [SerializeField] private Animator animator;
            [SerializeField] private DamagerSharkAnimatorEvents events;
            [SerializeField] private ParticleSystem damageParticles;
            [SerializeField] private ParticleSystem dripsParticles;
            [SerializeField] private ParticleSystem dripsUpsParticle;
            [SerializeField] private GameObject sharkMesh;

            public event Action OnDamageRaftAnimation;
            
            public void Init()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
                
                events.OnChangeLayerToDefault += EventsOnOnChangeLayerToDefault;
                events.OnChangeLayerToRaft += EventsOnOnChangeLayerToRaft;
                events.OnDamage += EventsOnOnDamage;
            }

            private void EventsOnOnDamage()
            {
                damageParticles.Play();
                OnDamageRaftAnimation?.Invoke();
            }

            private void EventsOnOnChangeLayerToRaft()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Raft"));
                dripsParticles.Play(true);
                dripsUpsParticle.Play(true);
            }

            private void EventsOnOnChangeLayerToDefault()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
                dripsParticles.Stop(true);
                damageParticles.Stop(true);
            }

            public void Die()
            {
                animator.SetTrigger("Die");
                animator.transform.DOScale(Vector3.zero, 4f);
            }
        }
    
        [SerializeField] private DamageSharkAnimator animatorManager;
        [SerializeField] private Range damageRange;
        public override void Init(RaftBase targetRaft, RaftDamagerService raftDamagerService)
        {
            base.Init(targetRaft, raftDamagerService);
            
            animatorManager.Init();

            animatorManager.OnDamageRaftAnimation += OnDamageRaftAnimation;
            
            transform.parent = targetRaft.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.parent = null;

            
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.forward))
            {
                RotateShark(Vector3Int.forward);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.back))
            {
                RotateShark(Vector3Int.back);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.right))
            {
                RotateShark(Vector3Int.right);
            }else
            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.left))
            {
                RotateShark(Vector3Int.left);
            }
            
            targetRaft.OnDeath += TargetRaftOnOnDeath;
        }

        private void TargetRaftOnOnDeath(DamageObject obj)
        {
            animatorManager.Die();
            Destroy(gameObject, 4);
        }

        private void OnDamageRaftAnimation()
        {
            TargetRaft.Damage(damageRange.RandomWithin());
        }

        public void RotateShark(Vector3Int target)
        {
            transform.rotation = Quaternion.LookRotation(TargetRaft.Coords - (TargetRaft.Coords + target));
        }
    }
}
