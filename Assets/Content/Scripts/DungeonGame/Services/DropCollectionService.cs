using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DropCollectionService : MonoBehaviour
    {
        [SerializeField] private List<DroppedItemBase> drops = new List<DroppedItemBase>(10);
        private GameDataObject gameDataObject;
        private PrefabSpawnerFabric spawnerFabric;
        private List<DroppedItemBase> dropListTmp = new List<DroppedItemBase>(5);
        private DungeonCharactersService charactersService;
        private DungeonResourcesService dungeonResourcesService;

        [Inject]
        private void Construct(GameDataObject gameDataObject, PrefabSpawnerFabric spawnerFabric, DungeonCharactersService charactersService, DungeonResourcesService dungeonResourcesService)
        {
            this.dungeonResourcesService = dungeonResourcesService;
            this.charactersService = charactersService;
            this.spawnerFabric = spawnerFabric;
            this.gameDataObject = gameDataObject;
        }

        public void SpawnDrop(RaftStorage.StorageItem item, Vector3 pos)
        {
            var drop = Instantiate(item.Item.GetDropPrefab(gameDataObject), pos + Vector3.up, Quaternion.identity);
            Destroy(drop.GetComponent<ActionsHolder>());
            spawnerFabric.InjectComponent(drop.GetComponent<DroppedItemBase>());
            drop
                .With(x => x.Animate())
                .With(x => x.SetItem(item));

            drops.Add(drop);
        }
        
        
        private void FixedUpdate()
        {
            dropListTmp.Clear();
            foreach (var sp in charactersService.SpawnedCharacters)
            {
                var charPos = sp.transform.position.RemoveY();
                for (var i = 0; i < drops.Count; i++)
                {
                    if (Vector3.Distance(charPos, drops[i].transform.position.RemoveY()) < 1f)
                    {
                        if (dungeonResourcesService.GetGlobalEmptySpace(drops[i].StorageItem))
                        {
                            dropListTmp.Add(drops[i]);
                            WorldPopupService.StaticSpawnPopup(charPos, drops[i].StorageItem);
                            dungeonResourcesService.AddItemsToAnyRafts(drops[i].StorageItem.Clone(), false);
                            break;
                        }
                    }
                }
            }

            if (dropListTmp.Count != 0)
            {
                for (int i = 0; i < dropListTmp.Count; i++)
                {
                    dropListTmp[i].DeleteItem();
                    drops.Remove(dropListTmp[i]);
                }
            }
        }
    }
}
