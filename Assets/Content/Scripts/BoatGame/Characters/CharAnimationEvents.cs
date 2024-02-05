using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.Characters
{
    public class CharAnimationEvents : MonoBehaviour
    {
        public event Action OnAttack;
        
        public void Attack()
        {
            OnAttack?.Invoke();
        }
    }
}
