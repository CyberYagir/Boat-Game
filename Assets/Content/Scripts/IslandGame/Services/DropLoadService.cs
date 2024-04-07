using System;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.IslandGame.Services
{
    public class DropLoadService : MonoBehaviour
    {

        [Inject]
        private void Construct(GameDataObject gameData, SaveDataObject saveData, PrefabSpawnerFabric prefabSpawnerFabric)
        {
            var dropped = saveData.GetTargetIsland().DroppedItems;
            for (int i = 0; i < dropped.Count; i++)
            {
                var item = gameData.GetItem(dropped[i].ItemID);
                if (!item) continue;
                
                
                
                var spawnedDrop = Instantiate(item.DropPrefab, dropped[i].Pos, Quaternion.Euler(dropped[i].Rot));
                spawnedDrop.LoadItem(dropped[i]);
                prefabSpawnerFabric.InjectComponent(spawnedDrop.gameObject);
            }
        }
    }
}
