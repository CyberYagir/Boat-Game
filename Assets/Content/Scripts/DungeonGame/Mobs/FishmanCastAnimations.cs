using UnityEngine;

namespace Content.Scripts.DungeonGame.Mobs
{
    public class FishmanCastAnimations : MonoBehaviour
    {
        [SerializeField] private ParticleSystem castParticles;

        public void StartCast()
        {
            castParticles.Play(true);
        }
        
        public void StopCast()
        {
            castParticles.Stop(true);
        }
    }
}
