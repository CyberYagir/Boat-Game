using System;
using StylizedWater2;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class FloatingShop : MonoBehaviour
    {
        [SerializeField] private FloatingTransform floatingTransform;
        [SerializeField] private Transform start, end;
        [SerializeField] private float speed;

        [Inject]
        private void Construct()
        {
            floatingTransform.transform.position = start.position;
        }

        private void Update()
        {
            floatingTransform.transform.position = Vector3.MoveTowards(floatingTransform.transform.position, end.position, Time.unscaledDeltaTime * speed);

            if (floatingTransform.transform.ToDistance(end) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
