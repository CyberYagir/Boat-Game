using System;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class FurnaceVisuals : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        private void Awake()
        {
            var furnace = GetComponent<Furnace>();
            furnace.OnFurnaceStateChange += OnChangeState;
            OnChangeState(furnace.FuelTicks > 0);
        }

        private void OnChangeState(bool state)
        {
            if (state)
            {
                particleSystem.Play(true);
            }
            else
            {
                particleSystem.Stop(true);
            }
        }
    }
}
