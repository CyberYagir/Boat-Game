using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class UrnCollectionService : MonoBehaviour
    {
        [SerializeField] private List<IDestroyable> demolished = new List<IDestroyable>(100);
        private DungeonCharactersService charactersService;

        [Inject]
        private void Construct(DungeonCharactersService charactersService, DungeonResourcesService dungeonResourcesService)
        {
            this.dungeonResourcesService = dungeonResourcesService;
            this.charactersService = charactersService;
        }

        public void AddUrn(IDestroyable urn)
        {
            demolished.Add(urn);
        }

        private List<IDestroyable> urnsListDemolishedTmp = new List<IDestroyable>(5);
        private DungeonResourcesService dungeonResourcesService;

        private void LateUpdate()
        {
            urnsListDemolishedTmp.Clear();
            foreach (var sp in charactersService.SpawnedCharacters)
            {
                var charPos = sp.transform.position.RemoveY();
                for (var i = 0; i < demolished.Count; i++)
                {
                    if (Vector3.Distance(charPos, demolished[i].transform.position.RemoveY()) < demolished[i].ActivationDistance)
                    {
                        urnsListDemolishedTmp.Add(demolished[i]);
                        demolished[i].Demolish(sp.transform.position);
                        if (demolished[i].DropTable)
                        {
                            var items = demolished[i].DropTable.GetItemsIterated(demolished[i].DropsCount);
                            foreach (var item in items)
                            {
                                if (dungeonResourcesService.GetGlobalEmptySpace(item))
                                {
                                    WorldPopupService.StaticSpawnPopup(demolished[i].transform.position, item);
                                    dungeonResourcesService.AddItemsToAnyRafts(item, false);
                                }
                            }
                        }

                        break;
                    }
                }
            }

            if (demolished.Count != 0)
            {
                for (int i = 0; i < urnsListDemolishedTmp.Count; i++)
                {
                    demolished.Remove(urnsListDemolishedTmp[i]);
                }
            }
            
        }
    }
}
