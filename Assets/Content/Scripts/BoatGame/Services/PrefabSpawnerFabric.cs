using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class PrefabSpawnerFabric : IFactory
    {
        private DiContainer container;

        [Inject]
        public PrefabSpawnerFabric(DiContainer container)
        {
            this.container = container;
        }


        public T SpawnItem<T>(T prefab, Vector3 pos, Quaternion rot, Transform parent) where T : Object
        {
            return container.InstantiatePrefabForComponent<T>(prefab, pos, rot, parent);
        }

        public GameObject SpawnItem(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            return container.InstantiatePrefab(prefab, pos, rot, parent);
        }
        
        
        public T SpawnItemOnGround<T>(T prefab, Vector3 pos, Quaternion rot, Transform parent) where T : Object
        {
            Vector3 spawnPos = pos;
            if (Physics.Raycast(pos + Vector3.up * 100, Vector3.down, out RaycastHit hit))
            {
                spawnPos = hit.point + Vector3.up;
            }

            var item = SpawnItem<T>(prefab, spawnPos, rot, parent);

            return item;
        }
    }
}
