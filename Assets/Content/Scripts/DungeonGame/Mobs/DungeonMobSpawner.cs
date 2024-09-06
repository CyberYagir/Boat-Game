using Content.Scripts.DungeonGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Mobs
{
    public class DungeonMobSpawner : MonoBehaviour
    {
        [SerializeField, Range(0, 1f)] private float spawnChance;
        [SerializeField] private DungeonMobObject.EMobDifficult difficult;
        
        
        public float SpawnChance => spawnChance;

        public DungeonMobObject.EMobDifficult Difficult => difficult;

        [Inject]
        private void Construct(MobsSpawnService mobsSpawnService)
        {
            mobsSpawnService.AddSpawner(this);
        }
    }
}
