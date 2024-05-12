using System;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class FurnaceVisuals : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        private void Awake()
        {
            GetComponent<Furnace>().OnFurnaceStateChange += OnChangeState;
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
