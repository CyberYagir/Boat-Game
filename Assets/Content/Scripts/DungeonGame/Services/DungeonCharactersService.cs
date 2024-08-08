using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class DungeonCharactersService : CharacterServiceBase, ICharacterService
    {
        [SerializeField] private DungeonCharacter prefab;
        [SerializeField] private List<DungeonCharacter> spawnedCharacters;
        public List<DungeonCharacter> SpawnedCharacters => spawnedCharacters;

        [Inject]
        private void Construct(
            SaveDataObject saveData,
            GameDataObject gameDataObject,
            PrefabSpawnerFabric prefabSpawnerFab,
            INavMeshProvider navMeshProvider,
            RoomsPlacerService roomsPlacerService,
            DungeonSelectionService selectionService
        )

        {
            for (int i = 0; i < saveData.Characters.Count; i++)
            {
                var character = saveData.Characters.GetCharacter(i);
                character.ClearEvents();

                Instantiate(prefab, roomsPlacerService.GetStartRoomRandomPos(), Quaternion.identity)
                    .With(x => x.Init(
                        character,
                        gameDataObject,
                        prefabSpawnerFab,
                        navMeshProvider,
                        saveData,
                        selectionService
                    ))
                    .With(x => SpawnedCharacters.Add(x));
            }
        }

        public override List<PlayerCharacter> GetSpawnedCharacters() => GetPlayers();

        private List<PlayerCharacter> tmpCharactersList = new List<PlayerCharacter>(5);
        public List<PlayerCharacter> GetPlayers()
        {
            tmpCharactersList.Clear();
            for (int i = 0; i < spawnedCharacters.Count; i++)
            {
                tmpCharactersList.Add(spawnedCharacters[i].PlayerCharacter);
            }
            return tmpCharactersList;
        }
    }
}
