using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame
{
    public class RandomAnimationStart : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void OnEnable()
        {
            animator.Play("Idle", 0, Random.value);
        }
    }
}
