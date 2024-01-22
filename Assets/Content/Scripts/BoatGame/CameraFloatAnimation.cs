using System;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class CameraFloatAnimation : MonoBehaviour
    {
        [SerializeField] private Transform animatedHolder;
        [SerializeField] private AnimationCurve floatCurve;
        [SerializeField] private float speed;
        private float time;

        private void Update()
        {
            time += Time.deltaTime * speed;
            if (time >= 1f)
            {
                time = 0;
            }
            animatedHolder.SetYLocalPosition(floatCurve.Evaluate(time));
        }
    }
}
