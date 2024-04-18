using System;
using UnityEngine;

namespace Content.Scripts.BoatGame
{
    public class DestroyParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particlePrefab;

        private void OnDestroy()
        {
            if (!this.gameObject.scene.isLoaded) return;
            Instantiate(particlePrefab, transform.position, Quaternion.identity).Play();
        }
    }
}
