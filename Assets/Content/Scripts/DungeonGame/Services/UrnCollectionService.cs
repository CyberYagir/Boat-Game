using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.PlayerActions;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Content.Scripts.IslandGame;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class UrnCollectionService : MonoBehaviour
    {
        [SerializeField] private List<IDestroyable> demolished = new List<IDestroyable>(100);
        
        private DungeonCharactersService charactersService;
        private System.Random rnd;
        private List<IDestroyable> urnsListDemolishedTmp = new List<IDestroyable>(5);
        private IResourcesService dungeonResourcesService;
        private DropCollectionService dropService;
        private DungeonService dungeonService;
        
        
        [Inject]
        private void Construct(
            DungeonCharactersService charactersService,
            IResourcesService dungeonResourcesService,
            GameDataObject gameDataObject,
            DropCollectionService dropService,
            DungeonService dungeonService
        )
        {
            this.dungeonService = dungeonService;
            this.dropService = dropService;
            this.dungeonResourcesService = dungeonResourcesService;
            this.charactersService = charactersService;

            rnd = new System.Random(dungeonService.Seed);
        }

        public void AddUrn(IDestroyable urn)
        {
            demolished.Add(urn);
        }



        private void FixedUpdate()
        {
            urnsListDemolishedTmp.Clear();
            foreach (var sp in charactersService.SpawnedCharacters)
            {
                var charPos = sp.transform.position.RemoveY();
                for (var i = 0; i < demolished.Count; i++)
                {
                    if (Vector3.Distance(charPos, demolished[i].transform.position.RemoveY()) < demolished[i].ActivationDistance)
                    {
                        if (demolished[i].IsCanDemolish())
                        {
                            urnsListDemolishedTmp.Add(demolished[i]);
                            demolished[i].Demolish(sp.transform.position);
                            var targetPos = demolished[i].transform.position;
                            if (demolished[i].DropTable)
                            {
                                var items = demolished[i].DropTable.GetItemsIterated(demolished[i].DropsCount);
                                int id = 0;
                                foreach (var item in items)
                                {
                                    if (dungeonResourcesService.GetGlobalEmptySpace(item))
                                    {
                                        if (items.Count > 1)
                                        {
                                            DOVirtual.DelayedCall(id / 3f, delegate { WorldPopupService.StaticSpawnPopup(targetPos, item); });
                                        }
                                        else
                                        {
                                            WorldPopupService.StaticSpawnPopup(targetPos, item);
                                        }

                                        dungeonResourcesService.AddItemsToAnyRafts(item.Clone(), false);
                                    }
                                    else
                                    {
                                        dropService.SpawnDrop(item, targetPos);
                                    }

                                    id++;
                                }
                            }

                            break;
                        }
                    }
                }
            }

            if (demolished.Count != 0)
            {
                for (int i = 0; i < urnsListDemolishedTmp.Count; i++)
                {
                    demolished.Remove(urnsListDemolishedTmp[i]);
                    dungeonService.AddDestroyedUrn(urnsListDemolishedTmp[i].UID);
                }
            }
            
        }

        public string GetNextGuid()
        {
            return Extensions.GenerateSeededGuid(rnd).ToString();
        }
    }
}
