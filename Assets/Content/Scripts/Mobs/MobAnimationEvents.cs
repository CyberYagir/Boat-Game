using System;
using UnityEngine;

namespace Content.Scripts.Mobs
{
    public class MobAnimationEvents : MonoBehaviour
    {
        public event Action OnAttack;


        public void Attack()
        {
            OnAttack?.Invoke();
        }
    }
}
