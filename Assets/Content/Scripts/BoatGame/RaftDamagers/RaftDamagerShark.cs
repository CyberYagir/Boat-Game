using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Mobs;
using Content.Scripts.BoatGame.PlayerActions;
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
            [SerializeField] private BoxCollider boxCollider;

            private bool isDieAnimationStarted;
            public event Action OnDamageRaftAnimation;
            
            public void Init()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
                boxCollider.enabled = false;
                
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
                boxCollider.enabled = true;
            }

            private void EventsOnOnChangeLayerToDefault()
            {
                sharkMesh.ChangeLayerWithChilds(LayerMask.NameToLayer("Default"));
                dripsParticles.Stop(true);
                damageParticles.Stop(true);
                boxCollider.enabled = false;
            }

            public void Die()
            {
                if (!isDieAnimationStarted)
                {
                    animator.SetTrigger("Die");
                    animator.transform.DOScale(Vector3.zero, 4f).SetLink(animator.gameObject);
                    isDieAnimationStarted = false;
                }
            }

            public void LoadAnimation()
            {
                animator.Play("DamageRaft");
                EventsOnOnChangeLayerToRaft();
            }

            public void DisableCollider()
            {
                boxCollider.enabled = false;
            }
        }
    
        [SerializeField] private DamageSharkAnimator animatorManager;
        [SerializeField] private Mob_Shark damageObject;
        [SerializeField] private Range damageRange;

        public override void Init(int id, RaftBase targetRaft, RaftDamagerService raftDamagerService)
        {
            base.Init(id, targetRaft, raftDamagerService);

            animatorManager.Init();

            damageObject.Init();

            damageObject.OnDeath += TargetRaftOnOnDeath;
            targetRaft.OnDeath += TargetRaftOnOnDeath;
            animatorManager.OnDamageRaftAnimation += OnDamageRaftAnimation;

            transform.parent = targetRaft.transform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.parent = null;

            if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.right))
            {
                RotateShark(Vector3Int.right);
            }
            else if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.left))
            {
                RotateShark(Vector3Int.left);
            }
            else if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.back))
            {
                RotateShark(Vector3Int.back);
            }
            else if (DamagerService.IsEmpty(TargetRaft.Coords, Vector3Int.forward))
            {
                RotateShark(Vector3Int.forward);
            }
        }

        private void TargetRaftOnOnDeath(DamageObject obj)
        {
            Die();
        }

        private void Die()
        {
            animatorManager.Die();
            TargetRaft.OnDeath -= TargetRaftOnOnDeath;
            GetComponent<ActionsHolder>().DisableSelection();
            EndDamager();
            Destroy(gameObject, 4);
        }

        private void OnDamageRaftAnimation()
        {
            var damage = damageRange.RandomWithin();
            if (damage >= TargetRaft.Health)
            {
                animatorManager.DisableCollider();
            }
            TargetRaft.Damage(damage, gameObject);
        }

        public void RotateShark(Vector3Int target)
        {
            transform.rotation = Quaternion.LookRotation(TargetRaft.Coords - (TargetRaft.Coords + target));
        }


        public override List<RaftDamagerDataKey> GetKeysData()
        {
            return new List<RaftDamagerDataKey>()
            {
                new("hp", damageObject.Health.ToString())
            };
        }
        public override void SetKeysData(List<RaftDamagerDataKey> data)
        {
            damageObject.SetHealth(float.Parse(data.Find(x => x.Key == "hp").Data));

            animatorManager.LoadAnimation();
        }
    }
}
