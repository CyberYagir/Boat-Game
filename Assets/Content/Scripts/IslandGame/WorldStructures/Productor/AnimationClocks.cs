using System;
using System.Collections;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures.Productor
{
    public class AnimationClocks : MonoBehaviour, IVisualAnimation
    {
        [SerializeField] private Transform hours, minutes, seconds;

        private void Awake()
        {
            StartCoroutine(Loop());
        }


        IEnumerator Loop()
        {
            while (true)
            {
                var now = DateTime.Now;


                seconds.localRotation = Quaternion.Euler(0f, 0, (now.Second / 60f) * 360);
                minutes.localRotation = Quaternion.Euler(0f, 0, (now.Minute / 60f) * 360);
                hours.localRotation = Quaternion.Euler(0f, 0, (now.Hour / 12f) * 360);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
