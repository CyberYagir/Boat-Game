using System;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class DestroyParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particlePrefab;

        private void OnDestroy()
        {
            Instantiate(particlePrefab, transform.position, Quaternion.identity).Play();
        }
    }
}
