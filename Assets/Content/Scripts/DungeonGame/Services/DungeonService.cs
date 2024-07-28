using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonService : MonoBehaviour
    {
        [SerializeField] private int seed;
        private Random random;

        public Random TargetRnd => random;

        [Inject]
        private void Construct()
        {
            random = new System.Random(seed);
        }
    }
}
