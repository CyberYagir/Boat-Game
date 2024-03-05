using UnityEngine;

namespace Content.Scripts.IslandGame
{
    public class MeshParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;


        public void Init(Mesh mesh)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                var shape = particleSystems[i].shape;

                shape.mesh = mesh;
            }
            
            particleSystems[0].Play(true);
        }

        public void DestroyAfter(int i)
        {
            Destroy(gameObject, i);
        }
    }
}
