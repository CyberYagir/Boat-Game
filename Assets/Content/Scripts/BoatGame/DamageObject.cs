using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public abstract class DamageObject : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Damage Object Data")] 
        private float maxHealth;
        [SerializeField, FoldoutGroup("Damage Object Data"), ShowIf(nameof(IsPlaying))] 
        private float health;

        public event Action<float> OnDamage;
        public event Action<DamageObject> OnDeath;
        
        public float Health => health;
        
        
        protected void SetHealth()
        {
            health = maxHealth;
        }
        protected void SetHealth(float value)
        {
            health = value;
        }

        public virtual void Damage(float dmg)
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
        [Button()]
        public void Kill()
        {
            Damage(1000);
        }
        
        public bool IsPlaying() => Application.isPlaying;
    }
}