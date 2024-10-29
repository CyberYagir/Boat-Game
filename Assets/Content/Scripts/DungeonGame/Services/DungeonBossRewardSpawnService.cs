using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonBossRewardSpawnService : MonoBehaviour
    {
        [SerializeField] private ChestDestroyable chestDestroyable;
        [SerializeField] private Transform spawnPoint;
        private GameDataObject gameDataObject;
        private ChestDestroyable chest;
        private IResourcesService resourcesService;
        private PrefabSpawnerFabric prefabSpawnerFabric;

        [Inject]
        private void Construct(DungeonService dungeonService, PrefabSpawnerFabric prefabSpawnerFabric, DungeonEnemiesService enemiesService, GameDataObject gameDataObject, IResourcesService resourcesService)
        {
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.resourcesService = resourcesService;
            this.gameDataObject = gameDataObject;
            chest = Instantiate(chestDestroyable, spawnPoint.position, Quaternion.identity);
            if (!dungeonService.IsBossDead())
            {
                chest.gameObject.SetActive(false);
                
                enemiesService.OnBossSpawned += OnBossSpawned;
                
                chest.OnOpen += TryAddScroll;
            }
            else
            {
                prefabSpawnerFabric.InjectComponent(chest.gameObject);
            }
        }

        private void OnBossSpawned(DungeonMob boss)
        {
            boss.OnDeath += OnBossDeath;
        }

        private void OnBossDeath(DamageObject obj)
        {
            chest.transform.position = obj.transform.position;
            chest.gameObject.SetActive(true);                
            prefabSpawnerFabric.InjectComponent(chest.gameObject);

        }

        private void TryAddScroll()
        {
            if (Random.value <= gameDataObject.ConfigData.ScrollDropChance)
            {
                resourcesService.AddToAnyStorage(gameDataObject.ConfigData.LoreItem);
            }
        }
    }
}
