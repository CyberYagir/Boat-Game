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
                if (delta >= Double.PositiveInfinity || delta <= 0 || TimeService.TickRate == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(delta);
                    OnTick?.Invoke(delta);
                }
            }
        }

        public void SpeedUpTime()
        {
            ChangeTimeScale(10);
        }

        public void NormalTime()
        {
            ChangeTimeScale(1);
        }

        public void ChangeTimeScale(int value)
        {
            TimeService.ChangeTimeScale(value);
        }
    }
}
