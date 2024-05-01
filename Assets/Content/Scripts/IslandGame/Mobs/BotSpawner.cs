using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
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
        
        public void Init(GameDataObject gameData, PrefabSpawnerFabric spawner, TerrainBiomeSO biome)
        {
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
                for (float x = -radius; x < radius; x++)
                {
                    for (float y = -radius; y < radius; y++)
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
                Gizmos.DrawSphere(gizmosPoints[i], 0.2f);
            }
        }
#endif
        public Vector3 GetRandomPointInRange()
        {
            var point = Random.insideUnitSphere * radius;

            point.y = 10;
            if (Physics.Raycast(transform.position + point, Vector3.down, out RaycastHit hit))
            {
                return hit.point;
            }

            return transform.position;
        }

        public void SpawnItem(ItemObject item)
        {
            if (item != null)
            {
                if (item.DropPrefab)
                {
                    spawner.SpawnItemOnGround(item.DropPrefab, spawnedMob.transform.position + Random.insideUnitSphere, Quaternion.Euler(Random.insideUnitSphere));
                }
            }
        }

        public void RespawnByCooldown()
        {
            DOVirtual.DelayedCall(respawnCooldown.RandomWithin(), RespawnMob);
        }
        
        public void RespawnMob()
        {
            if (biomes.Contains(biome))
            {
                var mob = gameData.GetMob(mobType);
                spawnedMob = spawner.SpawnItemOnGround(mob.Prefab, GetRandomPointInRange(), Quaternion.identity, transform, LayerMask.GetMask("Default", "Terrain"), 0);
                spawnedMob.Init(this);
            }
        }
    }
}
