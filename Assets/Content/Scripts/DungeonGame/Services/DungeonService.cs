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

        public int Seed => seed;

        [Inject]
        private void Construct()
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            random = new System.Random(Seed);
        }
    }
}
