using UnityEngine;

namespace Content.Scripts.BoatGame.Characters.States
{
    public class CharActionMiningStone : CharActionChopTree
    {
        private ParticleSystem spawnedParticles;
        
        protected override void OnDamage()
        {
            spawnedParticles.Play(true);
        }

        protected override void StartAnimation()
        {
            Machine.AnimationManager.TriggerMineAnim();
            spawnedParticles = spawnedAxe.GetComponentInChildren<ParticleSystem>();
        }
    }
}
