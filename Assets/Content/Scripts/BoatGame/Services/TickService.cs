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
            print("Add Tick Service");
            StartCoroutine(Loop());  
            print("execute " + transform.name);
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

        public void ChangeTimeScale(int value)
        {
            TimeService.ChangeTimeScale(value);
        }
    }
}
