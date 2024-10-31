using System;
using Content.Scripts.IslandGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public interface IDamagable
    {
        void Damage(float dmg, GameObject sender);
    }

    public abstract class DamageObject : MonoBehaviour, IDamagable
    {
        [SerializeField, FoldoutGroup("Damage Object Data")] 
        private float maxHealth;
        [SerializeField, FoldoutGroup("Damage Object Data"), ShowIf(nameof(IsPlaying))] 
        private float health;
        [SerializeField, FoldoutGroup("Damage Object Data")]
        private Transform attackPoint;
        [SerializeField, FoldoutGroup("Damage Object Data")]
        private Transform heightPoint;
        
        public event Action<float> OnDamage;
        public event Action<DamageObject> OnDeath;
        public event Action OnAttackedStart;
        public event Action OnAttackedEnd;
        
        public float Health => health;
        public bool IsDead => health <= 0;

        public Transform AttackPoint => attackPoint == null ? transform : attackPoint.transform;

        public float MaxHealth => maxHealth;

        public Transform HeightPoint => heightPoint == null ? AttackPoint : heightPoint;

        protected void SetHealth()
        {
            health = MaxHealth;
        }
        public void SetHealth(float value)
        {
            health = value;
        }

        public virtual void Damage(float dmg, GameObject sender)
        {
            health -= dmg;
            OnDamage?.Invoke(health);

            if (health <= 0)
            {
                Death();
            }
        }

        public virtual void Death()
        {
            OnDeath?.Invoke(this);
        }
        [Button(), ShowIf(nameof(IsPlaying))]
        public void Kill()
        {
            Damage(1000, gameObject);
        }
        
        public bool IsPlaying() => Application.isPlaying;

        public void AttackStart()
        {
            OnAttackedStart?.Invoke();
        }

        public void AttackStop()
        {
            OnAttackedEnd?.Invoke();
        }
    }
}