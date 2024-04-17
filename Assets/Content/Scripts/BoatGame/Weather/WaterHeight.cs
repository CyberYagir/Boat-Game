using UnityEngine;

namespace Content.Scripts.BoatGame.Weather
{
    public class WaterHeight : MonoBehaviour
    {
        [SerializeField] private Transform floater;
        [SerializeField] private Vector3 offcet;
        private void FixedUpdate()
        {
            var pos = (floater.localPosition * -transform.localScale.y) + offcet;

            if (pos.y > 0)
            {
                pos.y = 0;
            }

            transform.position = pos;
        }
    }
}
