using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Characters;
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
            DungeonSelectionService selectionService,
            DungeonEnemiesService enemiesService
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
                        selectionService,
                        enemiesService
                    ))
                    .With(x => SpawnedCharacters.Add(x))
                    .With(x => x.PlayerCharacter.NeedManager.OnDeath += OnDeath)
                    .With(x => x.PlayerCharacter.NeedManager.OnDamaged += CalculateHelp);
            }
        }

        private void CalculateHelp()
        {
            foreach (var spawnedCharacter in spawnedCharacters)
            {
                if (spawnedCharacter.TargetEnemy == null)
                {
                    spawnedCharacter.UpdateAttackRange();
                    if (spawnedCharacter.TargetEnemy == null)
                    {
                        var playerWithTarget = spawnedCharacters.Find(x => x.TargetEnemy != null);
                        if (playerWithTarget)
                        {
                            spawnedCharacter.SetTarget(playerWithTarget.TargetEnemy);
                            spawnedCharacter.PlayerCharacter.ActiveAction(EStateType.Attack);
                        }
                    }
                }
            }
        }

        private void OnDeath(Character obj)
        {
            spawnedCharacters.RemoveAll(x => x.PlayerCharacter.Character == obj);
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
