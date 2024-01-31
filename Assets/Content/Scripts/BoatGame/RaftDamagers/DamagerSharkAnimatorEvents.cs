using System;
using UnityEngine;

namespace Content.Scripts.BoatGame.RaftDamagers
{
    public class DamagerSharkAnimatorEvents : MonoBehaviour
    {
        public event Action OnChangeLayerToRaft;
        public event Action OnChangeLayerToDefault;
        public event Action OnDamage;
        public void ChangeLayerToRaft()
        {
            OnChangeLayerToRaft?.Invoke();
        }

        public void ChangeLayerToDefault()
        {
            OnChangeLayerToDefault?.Invoke();
        }

        public void DamageRaft()
        {
            OnDamage?.Invoke();
        }
    }
}
