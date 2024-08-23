using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.BoatGame.UI;
using Content.Scripts.DungeonGame.Services;
using UnityEngine;

namespace Content.Scripts.DungeonGame.UI
{
    public class UISelectCharacterWindow : AnimatedWindow
    {
        [SerializeField] private UICharacterPreview previewPrefab;
        [SerializeField] private UICharacterPreviewItem item;
        
        private List<UICharacterPreview> peviewList = new List<UICharacterPreview>(5);
        private List<UICharacterPreviewItem> spawnedItems = new List<UICharacterPreviewItem>(5);
        private List<PlayerCharacter> characters = new List<PlayerCharacter>(5);
        private ICharacterService characterService;
        private DungeonSelectionService selectionService;
        private DungeonUIService uiService;

        public void Init(ICharacterService characterService, PrefabSpawnerFabric spawnerFabric, DungeonSelectionService selectionService, DungeonUIService uiService)
        {
            this.uiService = uiService;
            this.selectionService = selectionService;
            this.characterService = characterService;
            characters = new(characterService.GetSpawnedCharacters());
            for (var i = 0; i < characters.Count; i++)
            {
                var spawnedCharacter = characters[i];
                var preview = spawnerFabric.SpawnItem(previewPrefab, new Vector3(i * 5, 100, 0), Quaternion.identity, transform)
                    .With(x => peviewList.Add(x));
                Instantiate(item, item.transform.parent)
                    .With(x => spawnedItems.Add(x))
                    .With(x => x.Init(preview, spawnedCharacter.Character.Name, spawnedCharacter, this));

                spawnedCharacter.NeedManager.OnDeath += delegate(Character character) { Redraw(); };
            }

            item.gameObject.SetActive(false);
        }


        public override void ShowWindow()
        {
            base.ShowWindow();
            if (characters.Count == 1)
            {
                SelectCharacter(characters[0]);
                return;
            }
            Redraw();
        }

        private void Redraw()
        {
            for (var i = 0; i < peviewList.Count; i++)
            {
                peviewList[i].UpdateCharacterVisuals(characters[i].Character);
            }

            for (var i = 0; i < spawnedItems.Count; i++)
            {
                spawnedItems[i].gameObject.SetActive(!characters[i].NeedManager.IsDead);
            }
        }

        public void SelectCharacter(PlayerCharacter character)
        {
            selectionService.SetActiveCharacter(character);
            uiService.ShowCharactersWindow();
        }
    }
}
