using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ItemsSystem;
using DG.DemiLib;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame
{
    public class RaftFishing : RaftBase
    {
        [SerializeField] private Range getItemTicksRage;

        [SerializeField] private WaterSpawnItemsObject itemsData;
        [SerializeField, ReadOnly] private int maxTicks;
        [SerializeField, ReadOnly] private int regenerationTimer;
        [SerializeField] private int maxItemsInNet = 3;
        [SerializeField, Range(0f, 1f)] private float spawnChance;

        private PrefabSpawnerFabric prefabSpawnerFabric;

        private List<WaterItem> items = new List<WaterItem>(3);

        [Inject]
        private void Construct(TickService tickService, PrefabSpawnerFabric prefabSpawnerFabric)
        {
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            for (int i = 0; i < maxItemsInNet; i++)
            {
                items.Add(null);
            }
            ResetTimer();
            tickService.OnTick += OnTick;
        }

        private void ResetTimer()
        {
            maxTicks = (int) getItemTicksRage.RandomWithin();
            regenerationTimer = 0;
        }

        private void OnTick(float delta)
        {
            regenerationTimer++;

            if (regenerationTimer >= maxTicks)
            {
                var empty = items.FindIndex(x => x == null);
                if (empty != -1)
                {
                    if (Random.value <= spawnChance)
                    {
                        var spawned = prefabSpawnerFabric.SpawnItem(itemsData.GetRandomItem(), transform.position);
                        spawned.InitStaticItem();
                        spawned.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("Raft"));
                        if (spawned)
                        {
                            items[empty] = spawned;
                        }
                    }
                }
                ResetTimer();
            }
        }
    }
}