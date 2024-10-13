using System.Collections.Generic;
using System.Text;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Services;
using Content.Scripts.ItemsSystem;
using Content.Scripts.Mobs;
using Content.Scripts.Mobs.Mob;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame.Mobs
{
    public class BotSpawner : MonoBehaviour
    {
        [SerializeField] private MobObject.EMobType mobType;
        [SerializeField] private float radius;
        [SerializeField] private Range respawnCooldown;

        [SerializeField] private List<TerrainBiomeSO> biomes;

        private SpawnedMob spawnedMob;
        private PrefabSpawnerFabric spawner;
        private GameDataObject gameData;
        private TerrainBiomeSO biome;
        private IslandMobsService islandMobsService;

        public void Init(GameDataObject gameData, PrefabSpawnerFabric spawner, TerrainBiomeSO biome, IslandMobsService islandMobsService)
        {
            this.islandMobsService = islandMobsService;
            this.biome = biome;
            this.spawner = spawner;
            this.gameData = gameData;
            RespawnMob();
        }

#if UNITY_EDITOR
        private List<Vector3> gizmosPoints = new List<Vector3>();
        private Vector3 lastPos;
        private float lastRadius;

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, mobType.ToString().ToLower() + "Icon.png", false);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void OnDrawGizmosSelected()
        {
            if (lastPos != transform.position || lastRadius != radius)
            {
                gizmosPoints.Clear();
                for (float x = -radius; x < radius; x += 3)
                {
                    for (float y = -radius; y < radius; y += 3)
                    {
                        if (Physics.Raycast(transform.position + new Vector3(x, radius / 2f, y), Vector3.down, out RaycastHit hit, radius, ~0, QueryTriggerInteraction.Ignore))
                        {
                            if (Vector3.Distance(hit.point, transform.position) < radius)
                            {
                                gizmosPoints.Add(hit.point);
                            }
                        }
                    }
                }

                lastPos = transform.position;
                lastRadius = radius;
            }

            for (int i = 0; i < gizmosPoints.Count; i++)
            {
                Gizmos.DrawSphere(gizmosPoints[i], 0.4f);

            }
        }
#endif

        public Vector3 GetRandomPointInRange()
        {
            var point = Random.insideUnitSphere * radius;

            point.y = radius + 10f;

            if (Physics.Raycast(transform.position + point, Vector3.down, out RaycastHit obstacle, Mathf.Infinity, LayerMask.GetMask("Obstacle")))
            {
                return GetRandomPointInRange();
            }

            if (Physics.Raycast(transform.position + point, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {

                return hit.point;
            }

            return transform.position;
        }
        
        public void RespawnByCooldown()
        {
            DOVirtual.DelayedCall(respawnCooldown.RandomWithin(), RespawnMob).SetLink(gameObject);
        }

        public void RespawnMob()
        {
            if (biomes.Contains(biome))
            {
                spawnedMob = islandMobsService.AddMob(gameData.GetMob(mobType), GetRandomPointInRange(), transform);
                spawnedMob.Init(this);
            }
        }
    }
}
