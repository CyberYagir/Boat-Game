using UnityEngine;

namespace Content.Scripts.BoatGame.Weather
{
    public class WaterHeight : MonoBehaviour
    {
        [SerializeField] private Transform floater;
        [SerializeField] private Vector3 offcet;
        private void FixedUpdate()
        {
            transform.position = (floater.localPosition * -transform.localScale.y) + offcet ;
        }
    }
}
