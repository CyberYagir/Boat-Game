using System.Collections.Generic;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class UICharactersList : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private UICharactersListItem item;
        private CharacterService characterService;
        private List<UICharactersListItem> spawnedItems = new List<UICharactersListItem>();
        private TickService tickService;
        private SelectionService selectionService;
        
        
        public void Init(
            CharacterService characterService,
            TickService tickService,
            SelectionService selectionService
        )
        {
            this.selectionService = selectionService;
            this.tickService = tickService;
            this.characterService = characterService;

            characterService.OnCharactersChange += OnCharacterChange;
            OnCharacterChange();
        }

        private void OnCharacterChange()
        {
            item.gameObject.SetActive(true);

            for (int i = 0; i < spawnedItems.Count; i++)
            {
                if (spawnedItems[i].TargetCharacter == null)
                {
                    spawnedItems[i].gameObject.SetActive(false);
                    spawnedItems[i].transform.SetAsLastSibling();
                }
            }

            for (int i = spawnedItems.Count; i < characterService.SpawnedCharacters.Count; i++)
            {
                Instantiate(item, item.transform.parent)
                    .With(x => spawnedItems.Add(x));
            }

            item.gameObject.SetActive(false);
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].gameObject.SetActive(i < characterService.SpawnedCharacters.Count);
                if (spawnedItems[i].gameObject.activeInHierarchy)
                {
                    spawnedItems[i].Init(characterService.SpawnedCharacters[i], tickService, selectionService, characterService, camera);
                }
            }
        }

        public PlayerCharacter GetCharacterUnderMouse()
        {
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                if (spawnedItems[i].gameObject.activeInHierarchy)
                {
                    if (spawnedItems[i].IsOver)
                    {
                        return spawnedItems[i].TargetCharacter;
                    }
                }
            }

            return null;
        }
    }
}
