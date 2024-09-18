using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class EventSpawnService : MonoBehaviour
    {
        [SerializeField] private GameObject shopEventPrefab;


        [Inject]
        private void Construct(SaveDataObject saveDataObject, PrefabSpawnerFabric spawnerFabric)
        {
            if (!saveDataObject.Global.ShopEventCreated)
            {
                if (saveDataObject.CrossGame.SoulsCount > 0)
                {
                    spawnerFabric.SpawnItem(shopEventPrefab, null);
                    saveDataObject.Global.CreateShopEvent();
                }
            }
        }
    }
}
