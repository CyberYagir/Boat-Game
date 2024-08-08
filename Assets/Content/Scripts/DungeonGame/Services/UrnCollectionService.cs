using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class UrnCollectionService : MonoBehaviour
    {
        [SerializeField] private List<UrnDestroyable> urnsList = new List<UrnDestroyable>(100);
        private DungeonCharactersService charactersService;

        [Inject]
        private void Construct(DungeonCharactersService charactersService)
        {
            this.charactersService = charactersService;
        }

        public void AddUrn(UrnDestroyable urn)
        {
            urnsList.Add(urn);
        }

        private List<UrnDestroyable> urnsListDemolishedTmp = new List<UrnDestroyable>(5);
        private void LateUpdate()
        {
            urnsListDemolishedTmp.Clear();
            foreach (var sp in charactersService.SpawnedCharacters)
            {
                var charPos = sp.transform.position.RemoveY();
                for (var i = 0; i < urnsList.Count; i++)
                {
                    if (Vector3.Distance(charPos, urnsList[i].transform.position.RemoveY()) < 1f)
                    {
                        urnsListDemolishedTmp.Add(urnsList[i]);
                        urnsList[i].Demolish(sp.transform.position);
                        break;
                    }
                }
            }

            if (urnsList.Count != 0)
            {
                for (int i = 0; i < urnsListDemolishedTmp.Count; i++)
                {
                    urnsList.Remove(urnsListDemolishedTmp[i]);
                }
            }
            
        }
    }
}
