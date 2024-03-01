using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public class CharAnimationEvents : MonoBehaviour
    {
        public event Action OnAttack;
        public event Action OnChop;
        
        public void Attack()
        {
            OnAttack?.Invoke();
        }

        public void Chop()
        {
            OnChop?.Invoke();
        }
    }
}
