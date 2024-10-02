using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Natives;
using Content.Scripts.Mobs;
using Content.Scripts.Mobs.Mob;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Services
{
    public class IslandMobsService : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<SpawnedMob> mobs = new List<SpawnedMob>();
        private PrefabSpawnerFabric prefabSpawnerFabric;
        private GameDataObject gameDataObject;

        [Inject]
        private void Construct(PrefabSpawnerFabric prefabSpawnerFabric, GameDataObject gameDataObject)
        {
            this.gameDataObject = gameDataObject;
            this.prefabSpawnerFabric = prefabSpawnerFabric;
        }

        public SpawnedMob AddMob(MobObject mob, Vector3 pos, Transform parent)
        {
            return AddMob<SpawnedMob>(mob.Prefab, pos, LayerMask.GetMask("Default", "Terrain"), parent);
        }
        public SpawnedMob AddMob(MobObject mob, Vector3 pos, int layer, Transform parent)
        {
            return AddMob<SpawnedMob>(mob.Prefab, pos, layer, parent);
        }
        
        public SpawnedMob AddMob(System.Random rnd, MobObject mob, Vector3 pos, int layer, Transform parent)
        {
            return AddMob<SpawnedMob>(mob.PrefabBySeed(rnd), pos, layer, parent);
        }
        
        public NativeController AddMob(NativeController prefab, Vector3 pos, int layer, Transform parent)
        {
            return AddMob<NativeController>(prefab, pos, layer, parent);
        }

        public T AddMob<T>(T mob, Vector3 pos, int layerMask, Transform parent) where T : SpawnedMob
        {
            var spawned = prefabSpawnerFabric.SpawnItemOnGround(mob, pos, Quaternion.identity, parent, layerMask, 0);
            spawned.OnSpawnDrop += OnMobDeath;
            mobs.Add(spawned);

            return spawned;
        }

        private void OnMobDeath(DamageObject obj)
        {
            var mob = obj as SpawnedMob;
            var item = mob.MobDropTable.GetItem();

            if (!mob.IsMomentalDeath)
            {
                prefabSpawnerFabric.SpawnItemOnGround(
                        item.GetDropPrefab(gameDataObject),
                        obj.transform.position + Random.insideUnitSphere,
                        Quaternion.Euler(Random.insideUnitSphere))
                    .With(x => x.SetItem(item));
            }

            mobs.Remove(mob);
        }


    }
}
