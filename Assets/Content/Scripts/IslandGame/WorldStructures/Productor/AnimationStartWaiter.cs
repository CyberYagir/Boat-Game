using System;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.IslandGame.WorldStructures.Productor
{
    public class AnimationStartWaiter : MonoBehaviour, IVisualAnimation
    {
        [SerializeField] private Animator animation;
        [SerializeField] private float delay;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private AnimationConveer conveer;
        [SerializeField] private ParticleSystem particleSystem;

        private void Awake()
        {
            animation.enabled = false;
        }


        private void Start()
        {
            animation.enabled = false;
            DOVirtual.DelayedCall(delay, delegate
            {
                animation.enabled = true;
            });
        }

        public void AnimationTriggered()
        {
            particleSystem.Play(true);
            conveer.ActivateItems(boxCollider);
        }
    }
}
