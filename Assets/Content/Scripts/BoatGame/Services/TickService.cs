using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class TickService : MonoBehaviour
    {
        public event Action<float> OnTick;

        [Inject]
        private void Construct()
        {
            StartCoroutine(Loop());
        }


        IEnumerator Loop()
        {
            while (true)
            {
                float delta = 1f / (float) TimeService.TickRate;
                yield return new WaitForSeconds(delta);
                OnTick?.Invoke(delta);
                
            }
        }

        public void ChangeTimeScale(int value)
        {
            TimeService.ChangeTimeScale(value);
        }
    }
}
